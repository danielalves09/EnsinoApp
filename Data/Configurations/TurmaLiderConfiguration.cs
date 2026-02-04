using EnsinoApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnsinoApp.Data.Configurations;

public class TurmaLiderConfiguration : IEntityTypeConfiguration<TurmaLider>
{
    public void Configure(EntityTypeBuilder<TurmaLider> builder)
    {
        builder.ToTable("TurmasLideres");

        builder.HasKey(tl => new { tl.IdTurma, tl.IdUsuario });

        builder.HasOne(tl => tl.Turma)
               .WithMany(t => t.Lideres)
               .HasForeignKey(tl => tl.IdTurma)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tl => tl.Usuario)
               .WithMany(u => u.TurmasLider)
               .HasForeignKey(tl => tl.IdUsuario)
               .OnDelete(DeleteBehavior.Cascade);
    }
}