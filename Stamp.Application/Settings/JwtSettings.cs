namespace Stamp.Application.Settings;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public int TokenLifetimeMinutes { get; set; }
    public string Issuer { get; set; } = "StampApi";
    public string Audience { get; set; } = "StampClient";
}