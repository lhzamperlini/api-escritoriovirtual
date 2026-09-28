namespace EscritorioVirtual.Application.Common.Exceptions;

public class UnauthorizedException(string message = "Acesso não autorizado.") : Exception(message)
{
}
