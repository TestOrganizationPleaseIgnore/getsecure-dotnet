using System.CommandLine;
using Maratsh.GetSecure.Core;

namespace Maratsh.GetSecure.CLI;

/// <summary>
/// Command-line interface for GetSecure utility.
/// </summary>
public class Program
{
    public static async Task<int> Main(string[] args)
    {
        // Create the root command
        var rootCommand = new RootCommand("Utility for securing expiring links");

        // Create the secure command
        var secureCommand = new Command("secure", "Create a secure link");

        // Add arguments
        var baseLinkArgument = new Argument<string>("baseLink", "Base URL to secure");
        var secretArgument = new Argument<string>("secret", "Secret string shared with web server");
        var periodOption = new Option<int>(
            name: "--period",
            description: "Expiration period in days",
            getDefaultValue: () => 30);

        secureCommand.AddArgument(baseLinkArgument);
        secureCommand.AddArgument(secretArgument);
        secureCommand.AddOption(periodOption);

        // Set the handler for the secure command
        secureCommand.SetHandler((string baseLink, string secret, int period) =>
        {
            try
            {
                var secureLink = SecureLink.CreateSecureLink(baseLink, secret, period);
                Console.WriteLine(secureLink);
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                Environment.Exit(1);
            }
        }, baseLinkArgument, secretArgument, periodOption);

        // Create the validate command
        var validateCommand = new Command("validate", "Validate a secure link");

        var linkArgument = new Argument<string>("link", "Secure link to validate");
        var validateSecretArgument = new Argument<string>("secret", "Secret string shared with web server");

        validateCommand.AddArgument(linkArgument);
        validateCommand.AddArgument(validateSecretArgument);

        validateCommand.SetHandler((string link, string secret) =>
        {
            var isValid = SecureLink.ValidateSecureLink(link, secret);
            if (isValid)
            {
                Console.WriteLine("Link is valid and not expired.");
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("Link is invalid or expired.");
                Environment.Exit(1);
            }
        }, linkArgument, validateSecretArgument);

        // Add commands to root command
        rootCommand.AddCommand(secureCommand);
        rootCommand.AddCommand(validateCommand);

        // For backward compatibility, also handle the case where arguments are passed directly
        if (args.Length >= 2 && !args[0].StartsWith('-') && args[0] != "secure" && args[0] != "validate")
        {
            // Legacy mode: direct arguments like the Python version
            var legacyArgs = new List<string> { "secure" };
            legacyArgs.AddRange(args);
            args = legacyArgs.ToArray();
        }

        // Execute the command
        return await rootCommand.InvokeAsync(args);
    }
}
