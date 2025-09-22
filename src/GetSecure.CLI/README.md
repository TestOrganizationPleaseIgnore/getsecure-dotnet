# GetSecure.CLI

Command-line interface for creating and validating secure, time-limited links.

## Features

- **Command-Line Interface**: Easy-to-use CLI for secure link operations
- **Modern Commands**: Clean command structure with subcommands
- **Legacy Support**: Backward compatible with original Python version command format
- **Cross-Platform**: Runs on any platform that supports .NET 9.0

## Installation

```bash
dotnet add package GetSecure.CLI
```

## Usage

### Create a Secure Link

```bash
# Using the new command structure
dotnet run -- secure "http://example.com/secret_page.html" "your-secret-key" --period 30

# Legacy mode (backward compatible with Python version)
dotnet run -- "http://example.com/secret_page.html" "your-secret-key" --period 30
```

### Validate a Secure Link

```bash
dotnet run -- validate "http://example.com/secret_page.html?sha256=abc123&expires=1234567890" "your-secret-key"
```

### Command Line Options

- `baseLink`: The base URL to secure (required)
- `secret`: Secret string shared with the web server (required)
- `--period`: Expiration period in days (default: 30)

## Examples

### Basic Usage

```bash
# Create a secure link valid for 7 days
dotnet run -- secure "http://example.com/secret_page.html" "my-secret-key" --period 7

# Output: http://example.com/secret_page.html?sha256=ABC123...&expires=1234567890
```

### Relative URLs

```bash
# Works with relative URLs too
dotnet run -- secure "/secret_page.html" "my-secret-key" --period 30

# Output: /secret_page.html?sha256=ABC123...&expires=1234567890
```

### Validation

```bash
# Validate a link
dotnet run -- validate "http://example.com/secret_page.html?sha256=ABC123&expires=1234567890" "my-secret-key"

# Output: Link is valid and not expired. (exit code 0)
# or: Link is invalid or expired. (exit code 1)
```

## Dependencies

This package depends on:
- `GetSecure.Core`: Core library for secure link functionality
- `System.CommandLine`: Modern command-line parsing

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
