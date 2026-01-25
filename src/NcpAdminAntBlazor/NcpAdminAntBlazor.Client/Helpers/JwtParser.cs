using System.Security.Claims;
using System.Text.Json;

namespace NcpAdminAntBlazor.Client.Helpers;

public static class JwtParser
{
    private static readonly IReadOnlyDictionary<string, string> ClaimTypeMappings = new Dictionary<string, string>
    {
        ["sub"] = ClaimTypes.NameIdentifier,
        ["name"] = ClaimTypes.Name,
        ["email"] = ClaimTypes.Email,
        ["role"] = ClaimTypes.Role,
        ["roles"] = ClaimTypes.Role
    }.AsReadOnly();

    public static List<Claim> ParseClaimsFromJwt(string jwt)
    {
        if (string.IsNullOrWhiteSpace(jwt)) return [];


        try
        {
            var parts = jwt.Split('.');
            if (parts.Length != 3) return [];

            var payload = parts[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs == null) return [];

            var claims = new List<Claim>();
            foreach (var kvp in keyValuePairs)
            {
                if (kvp.Value is JsonElement { ValueKind: JsonValueKind.Array } element)
                {
                    claims.AddRange(element.EnumerateArray()
                        .Where(item => item.ValueKind != JsonValueKind.Null)
                        .Select(item => new Claim(kvp.Key, item.ToString())));
                }
                else
                {
                    var value = kvp.Value?.ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        claims.Add(new Claim(kvp.Key, value));
                    }
                }
            }

            MapStandardClaims(claims);

            return claims;
        }
        catch (Exception)
        {
            return [];
        }
    }

    private static void MapStandardClaims(List<Claim> claims)
    {
        foreach (var (jwtType, dotnetType) in ClaimTypeMappings)
        {
            var claim = claims.FirstOrDefault(c => c.Type == jwtType);
            if (claim != null)
            {
                claims.Add(new Claim(dotnetType, claim.Value));
            }
        }
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        if (string.IsNullOrWhiteSpace(base64))
            throw new ArgumentException("Base64 string cannot be empty", nameof(base64));

        base64 = base64.Replace('-', '+').Replace('_', '/');

        switch (base64.Length % 4)
        {
            case 2:
                base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
        }

        return Convert.FromBase64String(base64);
    }
}