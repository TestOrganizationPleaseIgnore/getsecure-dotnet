#!/bin/bash

# Example usage of GetSecure .NET

echo "=== GetSecure .NET Examples ==="
echo

echo "1. Create a secure link (new command format):"
echo "dotnet run -- secure 'http://example.com/secret_page.html' 'my-secret-key' --period 30"
dotnet run -- secure 'http://example.com/secret_page.html' 'my-secret-key' --period 30
echo

echo "2. Create a secure link (legacy format, backward compatible):"
echo "dotnet run -- 'http://example.com/secret_page.html' 'my-secret-key' --period 7"
dotnet run -- 'http://example.com/secret_page.html' 'my-secret-key' --period 7
echo

echo "3. Create a secure link with relative URL:"
echo "dotnet run -- '/secret_page.html' 'my-secret-key' --period 30"
dotnet run -- '/secret_page.html' 'my-secret-key' --period 30
echo

echo "4. Validate a secure link:"
SECURE_LINK=$(dotnet run -- secure 'http://example.com/secret_page.html' 'my-secret-key' --period 30)
echo "Generated link: $SECURE_LINK"
echo "Validating..."
dotnet run -- validate "$SECURE_LINK" 'my-secret-key'
echo

echo "5. Test with invalid link:"
echo "dotnet run -- validate 'http://example.com/secret_page.html?sha256=invalid&expires=1234567890' 'my-secret-key'"
dotnet run -- validate 'http://example.com/secret_page.html?sha256=invalid&expires=1234567890' 'my-secret-key'
echo

echo "=== Examples completed ==="
