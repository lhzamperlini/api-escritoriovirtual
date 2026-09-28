namespace EscritorioVirtual.Application.Common.Exceptions;

public class CascadeDeleteException : Exception
{
    public CascadeDeleteException(string message)
        : base(message)
    {
    }

    public CascadeDeleteException(string name, object key)
        : base($"Não é possível excluir o registro '{name}' ({key}) pois ele possui dependências (Cascade Delete Prevented).")
    {
    }
}
