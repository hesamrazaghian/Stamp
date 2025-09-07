namespace Stamp.Application.DTOs
{
    /// <summary>
    /// Standard structure for API error responses.
    /// </summary>
    public class ErrorResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new( );
        public string? TraceId { get; set; }
    }
}
