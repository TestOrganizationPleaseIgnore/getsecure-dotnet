# GetSecure.Core

Core library for creating secure, time-limited links compatible with nginx's `secure_link` module.

## Features

- **Secure Link Generation**: Create time-limited links with configurable expiration periods
- **SHA256 Security**: Uses SHA256 hashing instead of MD5 for improved security
- **Nginx Compatibility**: Fully compatible with nginx's `secure_link` module
- **Link Validation**: Built-in validation for existing secure links
- **Cross-Platform**: Runs on any platform that supports .NET 9.0

## Installation

```bash
dotnet add package GetSecure.Core
```

## Usage

```csharp
using Maratsh.GetSecure.Core;

// Create a secure link
var secureLink = SecureLink.CreateSecureLink(
    "http://example.com/secret_page.html", 
    "your-secret-key", 
    30 // days
);

// Validate a secure link
var isValid = SecureLink.ValidateSecureLink(secureLink, "your-secret-key");
```

## API Reference

### SecureLink.CreateSecureLink

Creates a secure link with expiration time.

**Parameters:**
- `baseLink` (string): The base URL to secure
- `secret` (string): Secret string shared with the web server
- `periodDays` (int): Expiration period in days (default: 30)

**Returns:** A signed link with SHA256 hash and expiration timestamp

**Exceptions:**
- `ArgumentException`: Thrown when baseLink or secret is null or empty

### SecureLink.ValidateSecureLink

Validates a secure link by checking the hash and expiration.

**Parameters:**
- `secureLink` (string): The secure link to validate
- `secret` (string): Secret string shared with the web server

**Returns:** True if the link is valid and not expired, false otherwise

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
