using EnsinoApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnsinoApp.Data.Configurations;

public class SupervisaoConfiguration : IEntityTypeConfiguration<Supervisao>
{
    public void Configure(EntityTypeBuilder<Supervisao> builder)
    {
        builder.ToTable("Supervisoes");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Nome).IsRequired();

        builder.HasOne(s => s.Campus)
               .WithMany(c => c.Supervisoes)
               .HasForeignKey(s => s.IdCampus)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Lideres)
               .WithOne(u => u.Supervisao)
               .HasForeignKey(u => u.IdSupervisao)
               .OnDelete(DeleteBehavior.SetNull);
    }
}