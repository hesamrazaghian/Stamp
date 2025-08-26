namespace Stamp.Application.Settings;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public int TokenLifetimeMinutes { get; set; }
}