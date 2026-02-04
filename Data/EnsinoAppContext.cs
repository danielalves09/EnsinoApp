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

    private readonly IConfiguration _configuration;
    public EnsinoAppContext(IConfiguration configuration)
    {
        _configuration = configuration;

    }

    public DbSet<Campus> Campuses => Set<Campus>();
    public DbSet<Supervisao> Supervisoes => Set<Supervisao>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Licao> Licoes => Set<Licao>();
    public DbSet<Turma> Turmas => Set<Turma>();
    public DbSet<TurmaLider> TurmasLideres => Set<TurmaLider>();
    public DbSet<Casal> Casais => Set<Casal>();
    public DbSet<RelatorioSemanal> Relatorios => Set<RelatorioSemanal>();


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("EnsinoAppConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CampusConfiguration());
        modelBuilder.ApplyConfiguration(new SupervisaoConfiguration());
        modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
        modelBuilder.ApplyConfiguration(new CursoConfiguration());
        modelBuilder.ApplyConfiguration(new LicaoConfiguration());
        modelBuilder.ApplyConfiguration(new TurmaConfiguration());
        modelBuilder.ApplyConfiguration(new TurmaLiderConfiguration());
        modelBuilder.ApplyConfiguration(new CasalConfiguration());
        modelBuilder.ApplyConfiguration(new RelatorioSemanalConfiguration());

    }
}