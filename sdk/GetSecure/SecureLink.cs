using System;
using System.Security.Cryptography;
using System.Text;

namespace Maratsh.GetSecure;

/// <summary>
/// Utility class for creating secure, time-limited links compatible with nginx secure_link module.
/// Uses SHA256 hashing instead of MD5 for better security.
/// </summary>
public static class SecureLink
{
    /// <summary>
    /// Creates a secure link with expiration time.
    /// </summary>
    /// <param name="baseLink">The base URL to secure</param>
    /// <param name="secret">Secret string shared with the web server</param>
    /// <param name="periodDays">Expiration period in days (default: 30)</param>
    /// <returns>A signed link with SHA256 hash and expiration timestamp</returns>
    /// <exception cref="ArgumentException">Thrown when baseLink or secret is null or empty</exception>
    public static string CreateSecureLink(string baseLink, string secret, int periodDays = 30)
    {
        if (string.IsNullOrWhiteSpace(baseLink))
            throw new ArgumentException("Base link cannot be null or empty", nameof(baseLink));
        
        if (string.IsNullOrWhiteSpace(secret))
            throw new ArgumentException("Secret cannot be null or empty", nameof(secret));

        if (periodDays <= 0)
            throw new ArgumentException("Period must be greater than 0", nameof(periodDays));

        // Parse the URL to get the path
        var uri = new Uri(baseLink, UriKind.RelativeOrAbsolute);
        var path = uri.IsAbsoluteUri ? uri.AbsolutePath : baseLink;

        // Calculate expiration timestamp
        var expires = DateTimeOffset.UtcNow.AddDays(periodDays).ToUnixTimeSeconds();

        // Create hash string in the format expected by nginx secure_link
        var hashString = $"{expires}{path} {secret}";

        // Generate SHA256 hash
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashString));
        
        // Convert to base64url encoding (compatible with nginx)
        var protectionString = Convert.ToBase64String(hashBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

        // Construct the protected link
        var separator = baseLink.Contains('?') ? "&" : "?";
        return $"{baseLink}{separator}sha256={protectionString}&expires={expires}";
    }

    /// <summary>
    /// Validates a secure link by checking the hash and expiration.
    /// </summary>
    /// <param name="secureLink">The secure link to validate</param>
    /// <param name="secret">Secret string shared with the web server</param>
    /// <returns>True if the link is valid and not expired, false otherwise</returns>
    public static bool ValidateSecureLink(string secureLink, string secret)
    {
        if (string.IsNullOrWhiteSpace(secureLink) || string.IsNullOrWhiteSpace(secret))
            return false;

        try
        {
            var uri = new Uri(secureLink);
            var query = ParseQueryString(uri.Query);
            
            var expiresStr = query.GetValueOrDefault("expires");
            var hash = query.GetValueOrDefault("sha256");

            if (string.IsNullOrEmpty(expiresStr) || string.IsNullOrEmpty(hash))
                return false;

            // Check if expired
            if (!long.TryParse(expiresStr, out var expires) || 
                DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expires)
                return false;

            // Recreate the hash
            var path = uri.AbsolutePath;
            var hashString = $"{expires}{path} {secret}";

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashString));
            var expectedHash = Convert.ToBase64String(hashBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');

            return hash.Equals(expectedHash, StringComparison.Ordinal);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Simple query string parser that returns a dictionary of key-value pairs.
    /// </summary>
    /// <param name="queryString">The query string to parse (without the '?')</param>
    /// <returns>Dictionary of query parameters</returns>
    private static Dictionary<string, string> ParseQueryString(string queryString)
    {
        var result = new Dictionary<string, string>();
        
        if (string.IsNullOrEmpty(queryString))
            return result;

        // Remove the leading '?' if present
        if (queryString.StartsWith('?'))
            queryString = queryString.Substring(1);

        var pairs = queryString.Split('&', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var pair in pairs)
        {
            var keyValue = pair.Split('=', 2);
            if (keyValue.Length == 2)
            {
                var key = Uri.UnescapeDataString(keyValue[0]);
                var value = Uri.UnescapeDataString(keyValue[1]);
                result[key] = value;
            }
        }

        return result;
    }
}