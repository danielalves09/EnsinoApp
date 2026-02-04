using EnsinoApp.Models.Entities;
using EnsinoApp.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnsinoApp.Data.Configurations;

public class RelatorioSemanalConfiguration : IEntityTypeConfiguration<RelatorioSemanal>
{
    public void Configure(EntityTypeBuilder<RelatorioSemanal> builder)
    {
        builder.ToTable("RelatoriosSemanais");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.DataRegistro).IsRequired();
        builder.Property(r => r.DataLicao).IsRequired();
        builder.Property(r => r.Observacoes).IsRequired();

        builder.HasOne(r => r.Casal)
               .WithMany(c => c.Relatorios)
               .HasForeignKey(r => r.IdCasal)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Turma)
               .WithMany(t => t.Relatorios)
               .HasForeignKey(r => r.IdTurma)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Licao)
               .WithMany(l => l.Relatorios)
               .HasForeignKey(r => r.IdLicao)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Usuario)
               .WithMany(u => u.Relatorios)
               .HasForeignKey(r => r.IdUsuario)
               .OnDelete(DeleteBehavior.Restrict);
    }
}