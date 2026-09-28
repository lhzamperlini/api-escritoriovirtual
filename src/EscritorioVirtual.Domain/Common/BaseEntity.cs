namespace EscritorioVirtual.Domain.Common;
public abstract class BaseEntity
{
	public int Id { get; protected set; }
	public Guid Guid { get; protected set; } = Guid.CreateVersion7();
	public DateTime DataCriacao { get; protected set; } = DateTime.Now;
	public DateTime? DataAtualizacao { get; protected set; }
	public bool Status { get; protected set; }
}

public abstract class BaseEntity<TKey> : BaseEntity
{
#pragma warning disable CS8618
	public new TKey Id { get; protected set; }
#pragma warning restore CS8618
}