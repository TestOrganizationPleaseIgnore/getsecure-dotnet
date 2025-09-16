using FluentAssertions;
using Maratsh.GetSecure.Core;
using Xunit;

namespace Maratsh.GetSecure.Core.Tests;

public class SecureLinkTests
{
    private const string TestSecret = "test-secret-key";
    private const string TestBaseLink = "http://example.com/secret_page.html";
    private const string TestRelativeLink = "/secret_page.html";

    [Fact]
    public void CreateSecureLink_WithValidParameters_ShouldReturnSecureLink()
    {
        // Act
        var result = SecureLink.CreateSecureLink(TestBaseLink, TestSecret, 30);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().StartWith(TestBaseLink);
        result.Should().Contain("sha256=");
        result.Should().Contain("expires=");
    }

    [Fact]
    public void CreateSecureLink_WithRelativeUrl_ShouldReturnSecureLink()
    {
        // Act
        var result = SecureLink.CreateSecureLink(TestRelativeLink, TestSecret, 30);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().StartWith(TestRelativeLink);
        result.Should().Contain("sha256=");
        result.Should().Contain("expires=");
    }

    [Fact]
    public void CreateSecureLink_WithCustomPeriod_ShouldReturnSecureLinkWithCorrectExpiration()
    {
        // Arrange
        var period = 7;

        // Act
        var result = SecureLink.CreateSecureLink(TestBaseLink, TestSecret, period);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("expires=");
        
        // Extract expires value and verify it's approximately 7 days from now
        var expiresStart = result.IndexOf("expires=") + 8;
        var expiresEnd = result.IndexOf("&", expiresStart);
        if (expiresEnd == -1) expiresEnd = result.Length;
        var expiresStr = result.Substring(expiresStart, expiresEnd - expiresStart);
        
        var expires = long.Parse(expiresStr);
        var expectedExpires = DateTimeOffset.UtcNow.AddDays(period).ToUnixTimeSeconds();
        var timeDifference = Math.Abs(expires - expectedExpires);
        
        // Allow 1 minute tolerance for test execution time
        timeDifference.Should().BeLessThan(60);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CreateSecureLink_WithInvalidBaseLink_ShouldThrowArgumentException(string baseLink)
    {
        // Act & Assert
        var action = () => SecureLink.CreateSecureLink(baseLink, TestSecret, 30);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Base link cannot be null or empty*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CreateSecureLink_WithInvalidSecret_ShouldThrowArgumentException(string secret)
    {
        // Act & Assert
        var action = () => SecureLink.CreateSecureLink(TestBaseLink, secret, 30);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Secret cannot be null or empty*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void CreateSecureLink_WithInvalidPeriod_ShouldThrowArgumentException(int period)
    {
        // Act & Assert
        var action = () => SecureLink.CreateSecureLink(TestBaseLink, TestSecret, period);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Period must be greater than 0*");
    }

    [Fact]
    public void CreateSecureLink_WithDefaultPeriod_ShouldUseDefaultValue()
    {
        // Act
        var result = SecureLink.CreateSecureLink(TestBaseLink, TestSecret);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("expires=");
    }

    [Fact]
    public void ValidateSecureLink_WithValidLink_ShouldReturnTrue()
    {
        // Arrange
        var secureLink = SecureLink.CreateSecureLink(TestBaseLink, TestSecret, 30);

        // Act
        var result = SecureLink.ValidateSecureLink(secureLink, TestSecret);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateSecureLink_WithInvalidSecret_ShouldReturnFalse()
    {
        // Arrange
        var secureLink = SecureLink.CreateSecureLink(TestBaseLink, TestSecret, 30);
        var invalidSecret = "invalid-secret";

        // Act
        var result = SecureLink.ValidateSecureLink(secureLink, invalidSecret);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateSecureLink_WithExpiredLink_ShouldReturnFalse()
    {
        // Arrange
        var expiredTimestamp = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeSeconds();
        var expiredLink = $"{TestBaseLink}?sha256=invalid&expires={expiredTimestamp}";

        // Act
        var result = SecureLink.ValidateSecureLink(expiredLink, TestSecret);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ValidateSecureLink_WithInvalidLink_ShouldReturnFalse(string link)
    {
        // Act
        var result = SecureLink.ValidateSecureLink(link, TestSecret);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ValidateSecureLink_WithNullOrEmptySecret_ShouldReturnFalse(string secret)
    {
        // Arrange
        var secureLink = SecureLink.CreateSecureLink(TestBaseLink, TestSecret, 30);

        // Act
        var result = SecureLink.ValidateSecureLink(secureLink, secret);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateSecureLink_WithMalformedLink_ShouldReturnFalse()
    {
        // Arrange
        var malformedLink = "not-a-valid-url";

        // Act
        var result = SecureLink.ValidateSecureLink(malformedLink, TestSecret);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateSecureLink_WithMissingParameters_ShouldReturnFalse()
    {
        // Arrange
        var linkWithoutParams = TestBaseLink;

        // Act
        var result = SecureLink.ValidateSecureLink(linkWithoutParams, TestSecret);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateSecureLink_WithInvalidExpiresFormat_ShouldReturnFalse()
    {
        // Arrange
        var linkWithInvalidExpires = $"{TestBaseLink}?sha256=abc123&expires=invalid";

        // Act
        var result = SecureLink.ValidateSecureLink(linkWithInvalidExpires, TestSecret);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CreateSecureLink_ShouldGenerateValidLinks_ForSameInputs()
    {
        // Act
        var link1 = SecureLink.CreateSecureLink(TestBaseLink, TestSecret, 30);
        var link2 = SecureLink.CreateSecureLink(TestBaseLink, TestSecret, 30);

        // Assert
        // Both should be valid
        SecureLink.ValidateSecureLink(link1, TestSecret).Should().BeTrue();
        SecureLink.ValidateSecureLink(link2, TestSecret).Should().BeTrue();
        
        // Both should contain the expected components
        link1.Should().Contain("sha256=");
        link1.Should().Contain("expires=");
        link2.Should().Contain("sha256=");
        link2.Should().Contain("expires=");
    }

    [Fact]
    public void CreateSecureLink_WithSpecialCharacters_ShouldWorkCorrectly()
    {
        // Arrange
        var linkWithSpecialChars = "http://example.com/path with spaces/secret_page.html";
        var secretWithSpecialChars = "secret with spaces and symbols!@#$%";

        // Act
        var result = SecureLink.CreateSecureLink(linkWithSpecialChars, secretWithSpecialChars, 30);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().StartWith(linkWithSpecialChars);
        result.Should().Contain("sha256=");
        result.Should().Contain("expires=");
        
        // Should be valid
        SecureLink.ValidateSecureLink(result, secretWithSpecialChars).Should().BeTrue();
    }

    [Fact]
    public void CreateSecureLink_WithQueryParameters_ShouldPreserveExistingQuery()
    {
        // Arrange
        var linkWithQuery = "http://example.com/secret_page.html?existing=param";

        // Act
        var result = SecureLink.CreateSecureLink(linkWithQuery, TestSecret, 30);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("existing=param");
        result.Should().Contain("sha256=");
        result.Should().Contain("expires=");
        result.Should().Contain("&"); // Should have & separator for additional params
    }
}
