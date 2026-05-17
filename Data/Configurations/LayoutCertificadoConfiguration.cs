using EnsinoApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnsinoApp.Data.Configurations;

public class LayoutCertificadoConfiguration : IEntityTypeConfiguration<LayoutCertificado>
{
       public void Configure(EntityTypeBuilder<LayoutCertificado> builder)
       {
              builder.ToTable("LayoutsCertificado");

              builder.HasKey(l => l.Id);

              builder.Property(l => l.Nome)
                     .IsRequired()
                     .HasMaxLength(200);

              builder.Property(l => l.Descricao)
                     .HasMaxLength(500);

              builder.Property(l => l.TemplateHtml)
                     .IsRequired();

              builder.Property(l => l.ImagemFundoUrl)
                     .HasMaxLength(500);

              builder.Property(l => l.Orientacao)
                     .IsRequired()
                     .HasMaxLength(20)
                     .HasDefaultValue("Landscape");

              builder.Property(l => l.Ativo)
                     .HasDefaultValue(true);

              builder.Property(l => l.DataCriacao)
                     .IsRequired();

              // Um layout pode estar associado a vários cursos
              builder.HasMany(l => l.Cursos)
                     .WithOne(c => c.LayoutCertificado)
                     .HasForeignKey(c => c.IdLayoutCertificado)
                     .OnDelete(DeleteBehavior.SetNull);
       }
}