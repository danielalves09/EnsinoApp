using EnsinoApp.Models.Entities;
using EnsinoApp.Models.Enums;

namespace EnsinoApp.ViewModels.Cursos
{
    public class CursoDashboardViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Descricao { get; set; } = null!;
        public bool Ativo { get; set; }
        public string NomeCampus { get; set; } = null!;

        // Resumo geral
        public int TotalTurmas { get; set; }
        public int TotalCasais { get; set; }
        public int CasaisAtivos { get; set; }

        // Filtro
        public StatusTurma? FiltroStatusTurma { get; set; }
        public StatusPresenca? FiltroStatusCasal { get; set; }
        public string? BuscaCasal { get; set; }

        public List<TurmaInfo> Turmas { get; set; } = new List<TurmaInfo>();

        public class TurmaInfo
        {
            public int Id { get; set; }
            public string NomeLider { get; set; } = null!;
            public string imgLider { get; set; } = null!;
            public DateTime DataInicio { get; set; }
            public DateTime DataFim { get; set; }
            public StatusTurma Status { get; set; }
            public List<CasalInfo> Casais { get; set; } = new List<CasalInfo>();
        }

        public class CasalInfo
        {
            public int Id { get; set; }
            public string NomeMarido { get; set; } = null!;
            public string NomeEsposa { get; set; } = null!;
            public StatusCasal StatusCasal { get; set; }
            public StatusPresenca StatusPresenca { get; set; }
        }
    }
}
