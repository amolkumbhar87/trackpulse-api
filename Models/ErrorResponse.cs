public class ErrorResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}