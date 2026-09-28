namespace EscritorioVirtual.Infrastructure.Configurations;
public class DatabaseOptions
{
	public const string SectionName = "Database";

	public string ConnectionString { get; init; } = string.Empty;
	public int MaxRetry { get; init; } = 3;
	public int CommandTimeout { get; init; } = 30;
}