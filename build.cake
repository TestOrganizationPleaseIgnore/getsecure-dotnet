#tool "nuget:?package=NuGet.CommandLine&version=6.8.0"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release"); // Default to Release for production builds
var projectName = Argument("project", "");
var outputDir = "./output/packages";
var solutionFile = "./GetSecure.sln";

// Detect project from Git tag if not specified
if (string.IsNullOrEmpty(projectName))
{
    try
    {
        var gitTag = EnvironmentVariable("GITHUB_REF_NAME") ?? "";
        if (!string.IsNullOrEmpty(gitTag))
        {
            Information($"Detected Git tag: {gitTag}");
            
            // Parse tag format: <projectname>-v<version> (any project name)
            var match = System.Text.RegularExpressions.Regex.Match(gitTag, @"^(.+)-v(.+)$");
            if (match.Success)
            {
                projectName = match.Groups[1].Value;
                var version = match.Groups[2].Value;
                Information($"Auto-detected project: {projectName}, version: {version}");
            }
        }
    }
    catch (Exception ex)
    {
        Information($"Could not auto-detect project from Git tag: {ex.Message}");
    }
}

// Helper function to get project path
string GetProjectPath(string projectName)
{
    // Look for project in /src/ folder
    var projectPath = $"./src/{projectName}/{projectName}.csproj";
    
    if (FileExists(projectPath))
    {
        return projectPath;
    }
    
    // Fallback: try to find any .csproj file with the project name in /src/
    var srcDir = "./src";
    if (DirectoryExists(srcDir))
    {
        var projectFiles = GetFiles($"{srcDir}/**/*{projectName}*.csproj");
        if (projectFiles.Any())
        {
            return projectFiles.First().FullPath;
        }
    }
    
    throw new Exception($"Project not found: {projectName}. Expected location: {projectPath}");
}

// Clean output directory
Task("Clean")
    .Does(() =>
    {
        if (DirectoryExists(outputDir))
        {
            DeleteDirectory(outputDir, new DeleteDirectorySettings { Recursive = true });
        }
        CreateDirectory(outputDir);
    });

// Generate solution file
Task("Generate-Solution")
    .Does(() =>
    {
        Information("Generating solution file...");
        
        // Remove existing solution file if it exists
        if (FileExists(solutionFile))
        {
            DeleteFile(solutionFile);
        }
        
        // Create new solution file using CLI command
        StartProcess("dotnet", new ProcessSettings
        {
            Arguments = "new sln --name GetSecure --output ."
        });
        
        // Add all projects from /src/ folder to solution
        var srcDir = "./src";
        if (DirectoryExists(srcDir))
        {
            var projectFiles = GetFiles($"{srcDir}/**/*.csproj");
            foreach (var projectFile in projectFiles)
            {
                StartProcess("dotnet", new ProcessSettings
                {
                    Arguments = $"sln {solutionFile} add {projectFile.FullPath}"
                });
            }
        }
        
        Information($"Solution file created: {solutionFile}");
    });

// Restore NuGet packages
Task("Restore")
    .IsDependentOn("Generate-Solution")
    .Does(() =>
    {
        Information("Restoring NuGet packages...");
        DotNetRestore(solutionFile);
    });

// Build the solution
Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        Information($"Building solution in {configuration} configuration...");
        DotNetBuild(solutionFile, new DotNetBuildSettings
        {
            Configuration = configuration,
            NoRestore = true
        });
    });

// Pack NuGet packages
Task("Pack")
    .IsDependentOn("Build")
    .Does(() =>
    {
        Information("Packing NuGet packages...");
        
        var packSettings = new DotNetPackSettings
        {
            Configuration = configuration,
            OutputDirectory = outputDir,
            NoBuild = true,
            NoRestore = true,
            IncludeSymbols = true,
            SymbolPackageFormat = "snupkg"
        };
        
        // Projects are expected to be pre-versioned in their project files
        
        if (string.IsNullOrEmpty(projectName))
        {
            // Pack all projects in solution
            Information("Packing all projects in solution...");
            DotNetPack(solutionFile, packSettings);
        }
        else
        {
            // Pack specific project
            var projectPath = GetProjectPath(projectName);
            Information($"Packing specific project: {projectName} ({projectPath})");
            DotNetPack(projectPath, packSettings);
        }
        
        Information($"Packages created in: {outputDir}");
    });

// Publish packages to NuGet (uses Release configuration by default)
Task("Publish")
    .IsDependentOn("Pack")
    .Does(() =>
    {
        var apiKey = EnvironmentVariable("NUGET_API_KEY");
        var source = EnvironmentVariable("NUGET_SOURCE") ?? "https://api.nuget.org/v3/index.json";
        
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("NUGET_API_KEY environment variable is required for publishing");
        }
        
        Information($"Publishing packages to: {source}");
        
        var allPackages = GetFiles($"{outputDir}/*.nupkg");
        var packages = allPackages;
        
        if (!string.IsNullOrEmpty(projectName))
        {
            // Filter packages for specific project
            packages = new FilePathCollection(
                allPackages.Where(p => p.GetFilename().ToString().StartsWith(projectName)),
                new PathComparer(IsRunningOnUnix())
            );
            Information($"Publishing packages for project: {projectName}");
        }
        
        foreach (var package in packages)
        {
            Information($"Publishing: {package.GetFilename()}");
            DotNetNuGetPush(package.FullPath, new DotNetNuGetPushSettings
            {
                ApiKey = apiKey,
                Source = source,
                SkipDuplicate = true
            });
        }
        
        Information("Publishing completed successfully!");
    });

// Default task
Task("Default")
    .IsDependentOn("Pack");

// Run tests
Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        Information("Running tests...");
        DotNetTest("./tests/", new DotNetTestSettings
        {
            Configuration = configuration,
            NoBuild = true,
            NoRestore = true,
            Loggers = new[] { "trx" },
            ResultsDirectory = "./output/test-results"
        });
    });

// Clean up generated files
Task("Clean-All")
    .Does(() =>
    {
        if (FileExists(solutionFile))
        {
            DeleteFile(solutionFile);
        }
        
        if (DirectoryExists(outputDir))
        {
            DeleteDirectory(outputDir, new DeleteDirectorySettings { Recursive = true });
        }
        
        // Clean build artifacts
        var cleanSettings = new DotNetCleanSettings
        {
            Configuration = configuration
        };
        
        // Clean all projects in /src/ folder
        var srcDir = "./src";
        if (DirectoryExists(srcDir))
        {
            var projectFiles = GetFiles($"{srcDir}/**/*.csproj");
            foreach (var projectFile in projectFiles)
            {
                DotNetClean(projectFile.FullPath, cleanSettings);
            }
        }
    });

RunTarget(target);