using EnsinoApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnsinoApp.Data.Configurations;

public class CursoConfiguration : IEntityTypeConfiguration<Curso>
{
    public void Configure(EntityTypeBuilder<Curso> builder)
    {
        builder.ToTable("Cursos");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Nome).IsRequired();
        builder.Property(c => c.Descricao).IsRequired();
        builder.Property(c => c.Ativo).HasDefaultValue(true);

        builder.HasOne(c => c.Campus)
               .WithMany(cam => cam.Cursos)
               .HasForeignKey(c => c.IdCampus)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Licoes)
               .WithOne(l => l.Curso)
               .HasForeignKey(l => l.IdCurso)
               .OnDelete(DeleteBehavior.Cascade);
    }
}