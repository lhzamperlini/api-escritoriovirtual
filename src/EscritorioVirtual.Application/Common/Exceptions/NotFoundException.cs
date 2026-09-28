namespace EscritorioVirtual.Application.Common.Exceptions;

public class NotFoundException : Exception
{
	public NotFoundException(string message) : base(message) { }

	public NotFoundException(string name, object key)
		: base($"{name} com identificador '{key}' não foi encontrado.") { }
}
