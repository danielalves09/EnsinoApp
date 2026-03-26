using EnsinoApp.Data.Configurations;
using EnsinoApp.Models.Entities;
using EnsinoApp.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EnsinoApp.Data;

public class EnsinoAppContext : IdentityDbContext<Usuario, IdentityRole<int>, int>
{
    // [ATUALIZADO] Construtor agora recebe DbContextOptions injetado pelo AddDbContextPool
    // A configuração da connection string foi movida para o Program.cs
    public EnsinoAppContext(DbContextOptions<EnsinoAppContext> options) : base(options)
    {
    }

    public DbSet<Campus> Campuses => Set<Campus>();
    public DbSet<Supervisao> Supervisoes => Set<Supervisao>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Licao> Licoes => Set<Licao>();
    public DbSet<Turma> Turmas => Set<Turma>();
    public DbSet<Casal> Casais => Set<Casal>();
    public DbSet<Matricula> Matriculas => Set<Matricula>();
    public DbSet<RelatorioSemanal> Relatorios => Set<RelatorioSemanal>();
    public DbSet<InscricaoOnline> InscricoesOnline => Set<InscricaoOnline>();

    // [REMOVIDO] OnConfiguring — configuração movida para Program.cs (necessário para AddDbContextPool)

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new CampusConfiguration());
        modelBuilder.ApplyConfiguration(new SupervisaoConfiguration());
        modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
        modelBuilder.ApplyConfiguration(new CursoConfiguration());
        modelBuilder.ApplyConfiguration(new LicaoConfiguration());
        modelBuilder.ApplyConfiguration(new CasalConfiguration());
        modelBuilder.ApplyConfiguration(new TurmaConfiguration());
        modelBuilder.ApplyConfiguration(new MatriculaConfiguration());
        modelBuilder.ApplyConfiguration(new RelatorioSemanalConfiguration());
        modelBuilder.ApplyConfiguration(new InscricaoOnlineConfiguration());
    }
}
