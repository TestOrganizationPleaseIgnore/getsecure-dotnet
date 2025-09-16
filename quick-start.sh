#!/bin/bash

# Quick start script for GetSecure .NET
# This script demonstrates the new NUKE build system

echo "=== GetSecure .NET Quick Start ==="
echo

echo "1. Building the solution..."
./build.sh Compile
if [ $? -ne 0 ]; then
    echo "Build failed!"
    exit 1
fi

echo
echo "2. Running tests..."
./build.sh Test
if [ $? -ne 0 ]; then
    echo "Tests failed!"
    exit 1
fi

echo
echo "3. Creating packages..."
./build.sh Pack
if [ $? -ne 0 ]; then
    echo "Packaging failed!"
    exit 1
fi

echo
echo "4. Testing the CLI..."
echo "Creating a secure link:"
dotnet run --project sdk/GetSecure.CLI/GetSecure.CLI.csproj -- secure "http://example.com/test" "secret-key" --period 7

echo
echo "=== Quick start completed successfully! ==="
echo "Packages are available in the 'output/packages' directory"
echo "For more information, see BUILD.md"
