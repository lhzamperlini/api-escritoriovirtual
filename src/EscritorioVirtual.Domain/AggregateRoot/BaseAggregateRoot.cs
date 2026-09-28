using EscritorioVirtual.Domain.Common;

namespace EscritorioVirtual.Domain.AggregateRoot;
public abstract class BaseAggregateRoot : BaseEntity
{
	public Guid? UsuarioCriacaoId { get; protected set; }
	public Usuario? UsuarioCriacao { get; protected set; }
	public Guid? UsuarioAtualizacaoId { get; protected set; }
	public Usuario? UsuarioAtualizacao { get; protected set; }
}

public abstract class BaseAggregateRoot<TKey> : BaseEntity<TKey>
{
	public Guid? UsuarioCriacaoId { get; protected set; }
	public Usuario? UsuarioCriacao { get; protected set; }
	public Guid? UsuarioAtualizacaoId { get; protected set; }
	public Usuario? UsuarioAtualizacao { get; protected set; }
}