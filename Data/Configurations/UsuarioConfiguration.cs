using EnsinoApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnsinoApp.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.Property(u => u.NomeMarido).IsRequired();
        builder.Property(u => u.NomeEsposa).IsRequired();
        builder.Property(u => u.Ativo).HasDefaultValue(true);

        builder.HasOne(u => u.Campus)
               .WithMany(c => c.Usuarios)
               .HasForeignKey(u => u.IdCampus)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Supervisao)
               .WithMany(s => s.Usuarios)
               .HasForeignKey(u => u.IdSupervisao)
               .OnDelete(DeleteBehavior.Restrict);
    }
}