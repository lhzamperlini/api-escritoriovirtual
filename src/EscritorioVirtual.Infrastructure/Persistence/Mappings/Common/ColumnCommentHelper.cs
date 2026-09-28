namespace EscritorioVirtual.Infrastructure.Persistence.Mappings.Common;
public static class ColumnCommentHelper
{
	public static string TableComment(string tableName) =>
		$"Tabela responsável por armazenar {tableName}";

	public static string ColumnNull(string columnName, bool dadoSensivel = false) =>
		$@"{columnName} / Nulo / LGPD: {(dadoSensivel ? "dado sensível" : "dado não sensível")}";

	public static string ColumnNotNull(string columnName, bool dadoSensivel = false) =>
		$@"{columnName} / Não Nulo / LGPD: {(dadoSensivel ? "dado sensível" : "dado não sensível")}";
}