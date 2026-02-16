using EnsinoApp.Models.Enums;

namespace EnsinoApp.Models.Entities;


public class Matricula
{
    public int Id { get; set; }

    public int IdCasal { get; set; }
    public int IdTurma { get; set; }

    public DateTime DataMatricula { get; set; }
    public DateTime? DataConclusao { get; set; }
    public bool CertificadoEmitido { get; set; }
    public string? CaminhoCertificado { get; set; }
    public string? CodigoValidacao { get; set; }

    public string? NomeGC { get; set; } // GC naquele curso
    public StatusMatricula Status { get; set; } = StatusMatricula.Ativa;

    public Casal Casal { get; set; } = null!;
    public Turma Turma { get; set; } = null!;

    public ICollection<RelatorioSemanal> Relatorios { get; set; } = new List<RelatorioSemanal>();
}