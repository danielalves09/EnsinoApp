namespace EnsinoApp.Models.Entities;

public class PeriodoInscricao
{
    public int Id { get; set; }
    public int IdCurso { get; set; }
    public int IdCampus { get; set; }

    public DateTime DataAbertura { get; set; }
    public DateTime DataEncerramento { get; set; }

    public int VagasTotal { get; set; }
    public int VagasOcupadas { get; set; }

    public bool Ativo { get; set; } = false;

    public Curso Curso { get; set; } = null!;
    public Campus Campus { get; set; } = null!;

    public int VagasRestantes => VagasTotal - VagasOcupadas;
    public bool AindaAberto => Ativo
        && DateTime.Now >= DataAbertura
        && DateTime.Now <= DataEncerramento
        && VagasRestantes > 0;
}