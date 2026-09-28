using EscritorioVirtual.Domain.AggregateRoot;
using EscritorioVirtual.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscritorioVirtual.Infrastructure.Persistence.Mappings.Common;

public abstract class BaseEntityConfiguration<TEntity>
		: IEntityTypeConfiguration<TEntity>
		where TEntity : BaseEntity
{
	public virtual void Configure(EntityTypeBuilder<TEntity> builder)
	{
		_ = builder.HasKey(x => x.Id);

		_ = builder.Property(x => x.Id)
				.ValueGeneratedOnAdd()
				.HasComment(ColumnCommentHelper.ColumnNotNull("Identificador único da entidade"));

		_ = builder.Property(x => x.Guid)
				.IsRequired()
				.HasColumnType("uuid")
				.HasComment(ColumnCommentHelper.ColumnNotNull("Chave única da entidade (UUID v7)"));

		_ = builder.Property(x => x.DataCriacao)
				.IsRequired()
				.HasColumnType("timestamp without time zone")
				.HasComment(ColumnCommentHelper.ColumnNotNull("Data de criação"));

		_ = builder.Property(x => x.DataAtualizacao)
				.IsRequired(false)
				.HasColumnType("timestamp without time zone")
				.HasComment(ColumnCommentHelper.ColumnNull("Data da última atualização"));

		_ = builder.Property(x => x.Status)
				.IsRequired()
				.HasDefaultValue(true)
				.HasComment(ColumnCommentHelper.ColumnNotNull("Status do registro"));

		_ = builder.HasIndex(x => x.Guid).IsUnique();

		ConfigureEntity(builder);
	}

	protected abstract void ConfigureEntity(
			EntityTypeBuilder<TEntity> builder);
}

public abstract class BaseEntityConfiguration<TEntity, TKey> : BaseEntityConfiguration<TEntity>
		where TEntity : BaseEntity<TKey>
{
	public override void Configure(EntityTypeBuilder<TEntity> builder)
	{
		base.Configure(builder);

		_ = builder.Property(x => x.Id)
				.ValueGeneratedOnAdd()
				.HasComment(ColumnCommentHelper.ColumnNotNull("Identificador único da entidade"));
	}
}


public abstract class BaseAggregateRootConfiguration<TEntity, TKey> : BaseEntityConfiguration<TEntity, TKey>
		where TEntity : BaseAggregateRoot<TKey>
{
	public override void Configure(EntityTypeBuilder<TEntity> builder)
	{
		base.Configure(builder);

		_ = builder.Property(x => x.UsuarioCriacaoId)
				.IsRequired()
				.HasComment(ColumnCommentHelper.ColumnNotNull("ID do usuário que criou o registro"));

		_ = builder.Property(x => x.UsuarioAtualizacaoId)
				.IsRequired(false)
				.HasComment(ColumnCommentHelper.ColumnNull("ID do usuário que atualizou o registro"));


		_ = builder.HasOne(x => x.UsuarioCriacao)
				.WithMany()
				.HasForeignKey(x => x.UsuarioCriacaoId)
				.OnDelete(DeleteBehavior.Restrict);

		_ = builder.HasOne(x => x.UsuarioAtualizacao)
				.WithMany()
				.HasForeignKey(x => x.UsuarioAtualizacaoId)
				.OnDelete(DeleteBehavior.Restrict);
	}
}

public abstract class BaseAggregateConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
		where TEntity : BaseAggregate
{
	public virtual void Configure(EntityTypeBuilder<TEntity> builder)
	{
		_ = builder.HasKey(x => x.Id);

		_ = builder.Property(x => x.Id)
				.ValueGeneratedOnAdd()
				.HasComment(ColumnCommentHelper.ColumnNotNull("Identificador único da entidade"));

		_ = builder.Property(x => x.DataCriacao)
				.IsRequired()
				.HasColumnType("timestamp without time zone")
				.HasComment(ColumnCommentHelper.ColumnNotNull("Data de criação da entidade"));

		_ = builder.Property(x => x.DataAtualizacao)
				.IsRequired(false)
				.HasColumnType("timestamp without time zone")
				.HasComment(ColumnCommentHelper.ColumnNull("Data da última atualização da entidade"));

		ConfigureEntity(builder);
	}

	protected abstract void ConfigureEntity(
			EntityTypeBuilder<TEntity> builder);
}