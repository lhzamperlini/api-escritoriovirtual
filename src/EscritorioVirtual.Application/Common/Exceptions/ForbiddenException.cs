namespace EscritorioVirtual.Application.Common.Exceptions;

public class ForbiddenException : Exception
{
	public ForbiddenException(string message = "Acesso proibido.") : base(message) { }
}
