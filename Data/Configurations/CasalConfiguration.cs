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
              builder.Property(c => c.TelefoneConjuge1).IsRequired();
              builder.Property(c => c.TelefoneConjuge2).IsRequired();
              builder.Property(c => c.EmailConjuge1).IsRequired();
              builder.Property(c => c.EmailConjuge2).IsRequired();

              builder.HasIndex(c => c.EmailConjuge1).IsUnique();
              builder.HasIndex(c => c.EmailConjuge2).IsUnique();

              builder.Property(c => c.Status).HasConversion<int>().HasDefaultValue(StatusCasal.Ativo);

              builder.HasOne(c => c.Campus)
                     .WithMany(cam => cam.Casais)
                     .HasForeignKey(c => c.IdCampus)
                     .OnDelete(DeleteBehavior.Restrict);


       }
}