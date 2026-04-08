using EnsinoApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnsinoApp.Data.Configurations;

public class PeriodoInscricaoConfiguration : IEntityTypeConfiguration<PeriodoInscricao>
{
    public void Configure(EntityTypeBuilder<PeriodoInscricao> builder)
    {
        builder.ToTable("PeriodosInscricao");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.DataAbertura).IsRequired();
        builder.Property(p => p.DataEncerramento).IsRequired();
        builder.Property(p => p.VagasTotal).IsRequired();
        builder.Property(p => p.VagasOcupadas).HasDefaultValue(0);
        builder.Property(p => p.Ativo).HasDefaultValue(false);

        // Ignora propriedades calculadas — não são colunas no banco
        builder.Ignore(p => p.VagasRestantes);
        builder.Ignore(p => p.AindaAberto);

        builder.HasOne(p => p.Curso)
               .WithMany()
               .HasForeignKey(p => p.IdCurso)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Campus)
               .WithMany()
               .HasForeignKey(p => p.IdCampus)
               .OnDelete(DeleteBehavior.Restrict);
    }
}