using EnsinoApp.Models.Enums;

namespace EnsinoApp.ViewModels.Turmas
{
    public class CasalMatriculadoViewModel
    {
        public string Nome { get; set; } = null!;
        public StatusPresenca Presenca { get; set; }
        public DateTime? UltimaLicao { get; set; }
        public Models.Entities.Matricula Matricula { get; set; } = null!;
    }
}
