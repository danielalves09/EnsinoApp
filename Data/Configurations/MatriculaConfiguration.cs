using EnsinoApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnsinoApp.Data.Configurations;

public class MatriculaConfiguration : IEntityTypeConfiguration<Matricula>
{
    public void Configure(EntityTypeBuilder<Matricula> builder)
    {
        builder.ToTable("Matriculas");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.DataMatricula).IsRequired();
        builder.Property(m => m.Status).IsRequired();

        builder.Property(m => m.NomeGC)
               .HasMaxLength(100);

        builder.HasOne(m => m.Casal)
               .WithMany(c => c.Matriculas)
               .HasForeignKey(m => m.IdCasal);

        builder.HasOne(m => m.Turma)
               .WithMany(t => t.Matriculas)
               .HasForeignKey(m => m.IdTurma);

        // 🔐 Impede matrícula duplicada do mesmo casal na mesma turma
        builder.HasIndex(m => new { m.IdCasal, m.IdTurma })
               .IsUnique();
    }
}