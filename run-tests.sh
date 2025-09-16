#!/bin/bash

# Test runner script for GetSecure .NET

echo "=== GetSecure .NET Test Runner ==="
echo

echo "Building solution..."
dotnet build
if [ $? -ne 0 ]; then
    echo "Build failed!"
    exit 1
fi

echo
echo "Running Core library tests..."
dotnet test tests/GetSecure.Core.Tests/ --verbosity normal
if [ $? -ne 0 ]; then
    echo "Core tests failed!"
    exit 1
fi

echo
echo "Running CLI tests..."
dotnet test tests/GetSecure.CLI.Tests/ --verbosity normal
if [ $? -ne 0 ]; then
    echo "CLI tests failed!"
    exit 1
fi

echo
echo "=== All tests completed successfully! ==="
