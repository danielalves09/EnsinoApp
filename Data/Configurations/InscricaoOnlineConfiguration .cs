

using EnsinoApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnsinoApp.Data.Configurations;

public class InscricaoOnlineConfiguration : IEntityTypeConfiguration<InscricaoOnline>
{
    public void Configure(EntityTypeBuilder<InscricaoOnline> builder)
    {
        builder.ToTable("InscricoesOnline");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.NomeMarido)
            .IsRequired();

        builder.Property(i => i.NomeEsposa)
            .IsRequired();

        builder.Property(i => i.TelefoneMarido)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(i => i.TelefoneEsposa)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(i => i.EmailMarido)
            .IsRequired();

        builder.Property(i => i.EmailEsposa)
            .IsRequired();

        builder.HasIndex(c => c.EmailMarido).IsUnique();
        builder.HasIndex(c => c.EmailEsposa).IsUnique();

        builder.Property(i => i.NomeGC)
            .HasMaxLength(100);

        builder.Property(i => i.ParticipaGC)
            .IsRequired();

        builder.Property(i => i.Processada)
            .IsRequired();

        builder.Property(i => i.DataInscricao)
            .IsRequired();

        // Relacionamento com Campus
        builder.HasOne(i => i.Campus)
            .WithMany()
            .HasForeignKey(i => i.IdCampus)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Curso)
            .WithMany()
            .HasForeignKey(i => i.IdCurso)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
