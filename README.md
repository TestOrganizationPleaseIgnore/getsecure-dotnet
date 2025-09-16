# GetSecure .NET

The **GetSecure .NET** enables .NET developers to easily create secure, time-limited links compatible with nginx's `secure_link` module. This utility provides a modern, secure alternative to traditional link sharing with configurable expiration periods and SHA256-based security.

## Features

- **Secure Link Generation**: Create time-limited links with configurable expiration periods
- **SHA256 Security**: Uses SHA256 hashing instead of MD5 for improved security
- **Nginx Compatibility**: Fully compatible with nginx's `secure_link` module
- **Link Validation**: Built-in validation for existing secure links
- **Modern CLI**: Clean command-line interface with subcommands
- **Cross-Platform**: Runs on any platform that supports .NET 9.0

## Getting Started

### Prerequisites

- .NET 9.0 or later

### Installation

#### Build from Source

```bash
git clone https://github.com/maratsh/getsecure-net.git
cd getsecure-net
dotnet build
```

#### Run the Application

```bash
# Navigate to the SDK directory
cd sdk/GetSecure

# Create a secure link
dotnet run -- secure "http://example.com/secret_page.html" "your-secret-key" --period 30

# Validate a secure link
dotnet run -- validate "http://example.com/secret_page.html?sha256=...&expires=..." "your-secret-key"
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

## Nginx Configuration

Configure your nginx server to use the secure links:

```nginx
location /secret_page.html {
    secure_link $arg_sha256,$arg_expires;
    secure_link_md5 "$secure_link_expires$uri your-secret-key";

    if ($secure_link = "") {
        return 403;
    }

    if ($secure_link = "0") {
        return 410;
    }
}
```

**Note**: The nginx configuration still uses `secure_link_md5` directive, but you'll need to configure nginx to use SHA256 instead of MD5. This may require a custom nginx build or module that supports SHA256.

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

## Differences from Python Version

1. **Hashing Algorithm**: Uses SHA256 instead of MD5 for better security
2. **Parameter Name**: Uses `sha256` parameter instead of `md5` in the generated URLs
3. **Command Structure**: Modern command-line interface with subcommands
4. **Validation**: Includes built-in link validation functionality
5. **Error Handling**: More robust error handling and validation

## Security Considerations

- Always use strong, unique secret keys
- Consider the expiration period carefully based on your use case
- Keep your secret keys secure and don't expose them in logs or version control
- SHA256 provides better security than MD5, but ensure your nginx configuration supports it

## Contributing

We welcome community contributions to GetSecure .NET! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details on how to get started.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

- **Documentation**: [GitHub Wiki](https://github.com/maratsh/getsecure-net/wiki)
- **Issues**: [GitHub Issues](https://github.com/maratsh/getsecure-net/issues)
- **Discussions**: [GitHub Discussions](https://github.com/maratsh/getsecure-net/discussions)

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for a list of changes and version history.
