using Microsoft.AspNetCore.Identity;

namespace EnsinoApp.Models.Entities
{
    public class Usuario : IdentityUser<int>
    {
        public int IdCampus { get; set; }

        // Para líderes de curso (casais)
        public string NomeMarido { get; set; } = null!;
        public string NomeEsposa { get; set; } = null!;

        // Supervisão (apenas para líderes)
        public int? IdSupervisao { get; set; }
        public Supervisao? Supervisao { get; set; }

        // Dados adicionais
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }

        // Relacionamentos
        public Campus Campus { get; set; } = null!;
        public ICollection<RelatorioSemanal> Relatorios { get; set; } = new List<RelatorioSemanal>();
        public ICollection<TurmaLider> TurmasLider { get; set; } = new List<TurmaLider>();
    }
}