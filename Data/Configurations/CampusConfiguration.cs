using EnsinoApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnsinoApp.Data.Configurations;

public class CampusConfiguration : IEntityTypeConfiguration<Campus>
{
    public void Configure(EntityTypeBuilder<Campus> builder)
    {
        builder.ToTable("Campuses");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Nome).IsRequired();
        builder.Property(c => c.Telefone).IsRequired();
        builder.Property(c => c.Rua).IsRequired();
        builder.Property(c => c.Numero).IsRequired();
        builder.Property(c => c.Bairro).IsRequired();
        builder.Property(c => c.Cidade).IsRequired();
        builder.Property(c => c.Estado).IsRequired();


    }
}