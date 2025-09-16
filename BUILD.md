# GetSecure .NET Build System

This project uses [NUKE](https://nuke.build/) as the build automation system, replacing the previous shell scripts with a more robust and cross-platform solution.

## Prerequisites

- .NET 9.0 SDK
- Git (for versioning)

## Available Targets

### Basic Targets

- `Clean` - Removes all build artifacts and output directories
- `Restore` - Restores NuGet packages for the solution
- `Compile` - Builds the solution
- `Test` - Runs all unit tests
- `Pack` - Creates NuGet packages
- `Publish` - Publishes packages to NuGet.org (requires API key)

### Composite Targets

- `Full` - Runs Clean, Test, and Pack in sequence
- `CI` - Alias for Full (intended for CI/CD pipelines)

## Usage

### Local Development

```bash
# Build the solution
./build.sh Compile

# Run tests
./build.sh Test

# Create packages
./build.sh Pack

# Full build (clean, test, pack)
./build.sh Full
```

### Publishing Packages

To publish packages to NuGet.org, you need to provide your API key:

```bash
# Publish packages
./build.sh Publish --nuget-api-key YOUR_API_KEY

# Or set environment variable
export NUKE_NUGET_API_KEY=YOUR_API_KEY
./build.sh Publish
```

### Configuration

The build system uses several configuration files:

- `global.json` - .NET SDK version specification
- `GitVersion.yml` - Git-based versioning configuration
- `nuke-config.json` - NUKE build configuration

## Versioning

The project uses [GitVersion](https://gitversion.net/) for automatic semantic versioning based on Git history and conventional commits. Version numbers are automatically generated and applied to assemblies and NuGet packages.

### Version Bump Messages

You can control version increments using commit messages:

- `+semver: major` - Bump major version (breaking changes)
- `+semver: minor` - Bump minor version (new features)
- `+semver: patch` - Bump patch version (bug fixes)
- `+semver: none` - No version bump

## Migration from Shell Scripts

The following shell scripts have been replaced by NUKE targets:

- `build.sh` → `./build.sh Compile`
- `run-tests.sh` → `./build.sh Test`

The new system provides:
- Better cross-platform support
- More reliable dependency management
- Integrated versioning
- Package publishing capabilities
- Better CI/CD integration

## CI/CD Integration

The build system is designed to work seamlessly with CI/CD pipelines. Use the `CI` target for automated builds:

```bash
./build.sh CI
```

This will run a complete build including cleaning, testing, and packaging.
