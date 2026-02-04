using EnsinoApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnsinoApp.Data.Configurations;

public class LicaoConfiguration : IEntityTypeConfiguration<Licao>
{
    public void Configure(EntityTypeBuilder<Licao> builder)
    {
        builder.ToTable("Licoes");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Titulo).IsRequired();
        builder.Property(l => l.Descricao).IsRequired();
        builder.Property(l => l.NumeroSemana).IsRequired();
        builder.Property(l => l.Ativa).HasDefaultValue(true);

        builder.HasOne(l => l.Curso)
               .WithMany(c => c.Licoes)
               .HasForeignKey(l => l.IdCurso)
               .OnDelete(DeleteBehavior.Cascade);
    }
}