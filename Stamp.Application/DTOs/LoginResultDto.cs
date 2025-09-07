namespace Stamp.Application.DTOs
{
    // DTO for returning login result with JWT token
    public class LoginResultDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
