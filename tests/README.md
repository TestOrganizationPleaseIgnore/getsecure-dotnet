# GetSecure .NET Tests

This directory contains comprehensive unit tests for the GetSecure .NET project, following AWS SDK testing patterns.

## Test Structure

### GetSecure.Core.Tests
Unit tests for the core library functionality:
- **SecureLinkTests.cs**: Comprehensive tests for the `SecureLink` class
  - Link creation with various parameters
  - Link validation with valid and invalid inputs
  - Error handling for invalid inputs
  - Edge cases and special characters
  - SHA256 hash generation and validation

### GetSecure.CLI.Tests
Unit tests for the command-line interface:
- **ProgramTests.cs**: Tests for CLI functionality
  - Command parsing and execution
  - Error handling for invalid commands
  - Integration with the core library
  - Help and version commands

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Core Tests Only
```bash
dotnet test tests/GetSecure.Core.Tests/
```

### Run CLI Tests Only
```bash
dotnet test tests/GetSecure.CLI.Tests/
```

### Using the Test Runner Script
```bash
./run-tests.sh
```

## Test Coverage

The tests provide comprehensive coverage including:

- ✅ **Happy Path Testing**: Valid inputs and expected outputs
- ✅ **Error Handling**: Invalid inputs and error conditions
- ✅ **Edge Cases**: Special characters, empty strings, boundary values
- ✅ **Security Testing**: Hash validation and expiration checking
- ✅ **Integration Testing**: CLI integration with core library
- ✅ **Command Line Testing**: Various command combinations and options

## Test Dependencies

- **xUnit**: Testing framework
- **FluentAssertions**: Fluent assertion library for readable tests
- **Microsoft.NET.Test.Sdk**: .NET test SDK
- **coverlet.collector**: Code coverage collection

## Test Results

All core functionality tests pass successfully, ensuring:
- Secure link generation works correctly
- Link validation is accurate
- Error handling is robust
- CLI commands function as expected
- Integration between components is seamless

## Contributing

When adding new features:
1. Add corresponding unit tests
2. Ensure all tests pass
3. Maintain test coverage above 90%
4. Follow existing test patterns and naming conventions
