using EnsinoApp.Models.Entities;
using EnsinoApp.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnsinoApp.Data.Configurations;

public class TurmaConfiguration : IEntityTypeConfiguration<Turma>
{
       public void Configure(EntityTypeBuilder<Turma> builder)
       {
              builder.ToTable("Turmas");

              builder.HasKey(t => t.Id);
              builder.Property(t => t.DataInicio).IsRequired();
              builder.Property(t => t.DataFim).IsRequired();
              builder.Property(t => t.Status).HasConversion<int>().HasDefaultValue(StatusTurma.Acomecar);

              builder.HasOne(t => t.Curso)
                     .WithMany(c => c.Turmas)
                     .HasForeignKey(t => t.IdCurso)
                     .OnDelete(DeleteBehavior.Restrict);

              builder.HasOne(t => t.Campus)
                     .WithMany(c => c.Turmas)
                     .HasForeignKey(t => t.IdCampus)
                     .OnDelete(DeleteBehavior.Restrict);

              builder.HasOne(t => t.Lider)
                     .WithMany(t => t.TurmasLideradas)
                     .HasForeignKey(t => t.IdLider)
                     .OnDelete(DeleteBehavior.Restrict);


       }
}