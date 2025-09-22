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

This project uses [NUKE](https://nuke.build/) as the build automation system. See [BUILD.md](BUILD.md) for detailed build instructions.

```bash
git clone https://github.com/maratsh/getsecure-net.git
cd getsecure-net


