using EnsinoApp.Models.Entities;
using EnsinoApp.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnsinoApp.Data.Configurations;

public class CasalConfiguration : IEntityTypeConfiguration<Casal>
{
    public void Configure(EntityTypeBuilder<Casal> builder)
    {
        builder.ToTable("Casais");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.NomeConjuge1).IsRequired();
        builder.Property(c => c.NomeConjuge2).IsRequired();
        builder.Property(c => c.Status).HasConversion<int>().HasDefaultValue(StatusCasal.Ativo);

        builder.HasOne(c => c.Campus)
               .WithMany(cam => cam.Casais)
               .HasForeignKey(c => c.IdCampus)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Turma)
               .WithMany(t => t.Casais)
               .HasForeignKey(c => c.IdTurma)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Relatorios)
               .WithOne(r => r.Casal)
               .HasForeignKey(r => r.IdCasal)
               .OnDelete(DeleteBehavior.Cascade);
    }
}