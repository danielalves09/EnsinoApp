using EnsinoApp.ViewModels.Casal;
using EnsinoApp.ViewModels.Inscricao;
using EnsinoApp.ViewModels.Turmas;

namespace EnsinoApp.ViewModels.Matricula
{
    public class MatriculaDashboardViewModel
    {
        // Cards de resumo
        public int TotalCasais { get; set; }
        public int TotalInscricoes { get; set; }
        public int InscricoesPendentes { get; set; }
        public int MatriculasAtivas { get; set; }
        public int TurmasAtivas { get; set; }

        // Listagens principais
        public List<InscricaoOnlineResumoViewModel> InscricoesPendentesLista { get; set; } = new();
        public List<CasalResumoViewModel> Casais { get; set; } = new();
        public List<TurmaResumoViewModel> Turmas { get; set; } = new();
    }
}
