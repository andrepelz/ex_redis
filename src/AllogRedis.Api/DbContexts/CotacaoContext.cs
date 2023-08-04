using AllogRedis.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace AllogRedis.Api.DbContexts;

public class CotacaoContext : DbContext
{
    public DbSet<Cotacao> Cotacao { get; set; } = null!;

    public CotacaoContext(DbContextOptions<CotacaoContext> options)
    : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cotacao>().ToTable("cotacao", t => t.ExcludeFromMigrations());

        CotacaoInitFluentApi(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void CotacaoInitFluentApi(ModelBuilder modelBuilder)
    {
        var cotacao = modelBuilder.Entity<Cotacao>();

        cotacao
            .HasKey(cotacao => cotacao.CotacaoId);

        cotacao
            .Property(cotacao => cotacao.CotacaoId)
            .HasColumnName("id");

        cotacao
            .Property(cotacao => cotacao.Sigla)
            .HasColumnName("sigla")
            .HasMaxLength(3)
            .IsRequired(false);

        cotacao
            .Property(cotacao => cotacao.NomeMoeda)
            .HasColumnName("nome_moeda")
            .HasMaxLength(40)
            .IsRequired(false);

        cotacao
            .Property(cotacao => cotacao.Data)
            .HasColumnName("data")
            .IsRequired();

        cotacao
            .Property(cotacao => cotacao.Valor)
            .HasColumnName("valor")
            .HasPrecision(18,2)
            .IsRequired();
    }
}