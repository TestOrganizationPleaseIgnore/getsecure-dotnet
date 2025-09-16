using FluentAssertions;
using Maratsh.GetSecure.CLI;
using Xunit;

namespace Maratsh.GetSecure.CLI.Tests;

public class ProgramTests
{
    [Fact]
    public async Task SecureCommand_WithValidArguments_ShouldCreateSecureLink()
    {
        // Arrange
        var args = new[] { "secure", "http://example.com/test", "secret", "--period", "30" };

        // Act
        var result = await Program.Main(args);

        // Assert
        result.Should().Be(0);
    }
}