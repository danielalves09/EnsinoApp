using EnsinoApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnsinoApp.Data.Configurations;

public class AgendaLicaoConfiguration : IEntityTypeConfiguration<AgendaLicao>
{
    public void Configure(EntityTypeBuilder<AgendaLicao> builder)
    {
        builder.ToTable("AgendaLicoes");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.DataAula).IsRequired();
        builder.Property(a => a.DiaSemana).HasConversion<int>().IsRequired();
        builder.Property(a => a.Local).HasMaxLength(300);
        builder.Property(a => a.Observacoes).HasMaxLength(1000);
        builder.Property(a => a.LembreteEnviado).HasDefaultValue(false);

        // Uma AgendaLicao pertence a uma Turma
        builder.HasOne(a => a.Turma)
               .WithMany(t => t.Agenda)
               .HasForeignKey(a => a.IdTurma)
               .OnDelete(DeleteBehavior.Cascade);

        // Uma AgendaLicao referencia uma Licao
        builder.HasOne(a => a.Licao)
               .WithMany()
               .HasForeignKey(a => a.IdLicao)
               .OnDelete(DeleteBehavior.Restrict);

        // Índice para a query do background service (busca por data + lembrete não enviado)
        builder.HasIndex(a => new { a.DataAula, a.LembreteEnviado });

        // Evita duplicata: mesma lição na mesma turma
        builder.HasIndex(a => new { a.IdTurma, a.IdLicao }).IsUnique();
    }
}