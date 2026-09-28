namespace EscritorioVirtual.Application.Common.Exceptions;

public class ErrorResponse
{
	public int StatusCode { get; init; }
	public string Message { get; init; } = string.Empty;
	public string? Details { get; init; }
	public DateTime Timestamp { get; init; } = DateTime.UtcNow;
	public string? TraceId { get; init; }
	public IDictionary<string, string[]>? Errors { get; init; }
}
