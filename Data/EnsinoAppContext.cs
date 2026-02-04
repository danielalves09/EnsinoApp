using EnsinoApp.Models.Entities;
using EnsinoApp.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EnsinoApp.Data;

public class EnsinoAppContext : IdentityDbContext<Usuario, IdentityRole<int>, int>
{
    public EnsinoAppContext(DbContextOptions<EnsinoAppContext> options) : base(options) { }

    public DbSet<Campus> Campuses => Set<Campus>();
    public DbSet<Supervisao> Supervisoes => Set<Supervisao>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Licao> Licoes => Set<Licao>();
    public DbSet<Turma> Turmas => Set<Turma>();
    public DbSet<TurmaLider> TurmasLideres => Set<TurmaLider>();
    public DbSet<Casal> Casais => Set<Casal>();
    public DbSet<RelatorioSemanal> Relatorios => Set<RelatorioSemanal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Campus
        modelBuilder.Entity<Campus>()
            .HasMany(c => c.Cursos)
            .WithOne(curso => curso.Campus)
            .HasForeignKey(curso => curso.IdCampus)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Campus>()
            .HasMany(c => c.Turmas)
            .WithOne(t => t.Campus)
            .HasForeignKey(t => t.IdCampus)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Campus>()
            .HasMany(c => c.Usuarios)
            .WithOne(u => u.Campus)
            .HasForeignKey(u => u.IdCampus)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Campus>()
            .HasMany(c => c.Casais)
            .WithOne(casal => casal.Campus)
            .HasForeignKey(casal => casal.IdCampus)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Campus>()
            .HasMany(c => c.Supervisoes)
            .WithOne(s => s.Campus)
            .HasForeignKey(s => s.IdCampus)
            .OnDelete(DeleteBehavior.Restrict);

        // Supervisão
        modelBuilder.Entity<Supervisao>()
            .HasMany(s => s.Lideres)
            .WithOne(u => u.Supervisao)
            .HasForeignKey(u => u.IdSupervisao)
            .OnDelete(DeleteBehavior.SetNull);

        // Curso
        modelBuilder.Entity<Curso>()
            .HasMany(c => c.Licoes)
            .WithOne(l => l.Curso)
            .HasForeignKey(l => l.IdCurso)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Curso>()
            .HasMany(c => c.Turmas)
            .WithOne(t => t.Curso)
            .HasForeignKey(t => t.IdCurso)
            .OnDelete(DeleteBehavior.Restrict);

        // Turma
        modelBuilder.Entity<Turma>()
            .HasMany(t => t.Casais)
            .WithOne(c => c.Turma)
            .HasForeignKey(c => c.IdTurma)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Turma>()
            .HasMany(t => t.Relatorios)
            .WithOne(r => r.Turma)
            .HasForeignKey(r => r.IdTurma)
            .OnDelete(DeleteBehavior.Restrict);

        // TurmaLider
        modelBuilder.Entity<TurmaLider>()
            .HasKey(tl => new { tl.IdTurma, tl.IdUsuario });

        modelBuilder.Entity<TurmaLider>()
            .HasOne(tl => tl.Turma)
            .WithMany(t => t.Lideres)
            .HasForeignKey(tl => tl.IdTurma)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TurmaLider>()
            .HasOne(tl => tl.Usuario)
            .WithMany(u => u.TurmasLider)
            .HasForeignKey(tl => tl.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        // Casal
        modelBuilder.Entity<Casal>()
            .HasMany(c => c.Relatorios)
            .WithOne(r => r.Casal)
            .HasForeignKey(r => r.IdCasal)
            .OnDelete(DeleteBehavior.Cascade);

        // Licao
        modelBuilder.Entity<Licao>()
            .HasMany(l => l.Relatorios)
            .WithOne(r => r.Licao)
            .HasForeignKey(r => r.IdLicao)
            .OnDelete(DeleteBehavior.Restrict);

        // Relatorio
        modelBuilder.Entity<RelatorioSemanal>()
            .HasOne(r => r.Usuario)
            .WithMany(u => u.Relatorios)
            .HasForeignKey(r => r.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict);

        // Enum mappings
        modelBuilder.Entity<Casal>()
            .Property(c => c.Status)
            .HasConversion<int>()
            .HasDefaultValue(StatusCasal.Ativo);

        modelBuilder.Entity<Turma>()
            .Property(t => t.Status)
            .HasConversion<int>()
            .HasDefaultValue(StatusTurma.Acomecar);

        // Default values
        modelBuilder.Entity<Usuario>()
            .Property(u => u.Ativo)
            .HasDefaultValue(true);

        modelBuilder.Entity<Curso>()
            .Property(c => c.Ativo)
            .HasDefaultValue(true);

        modelBuilder.Entity<Licao>()
            .Property(l => l.Ativa)
            .HasDefaultValue(true);
    }
}