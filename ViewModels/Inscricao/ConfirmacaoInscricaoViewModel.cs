using System;

namespace EnsinoApp.ViewModels.Inscricao
{
    public class ConfirmacaoInscricaoViewModel
    {
        public string NomeMarido { get; set; } = null!;
        public string NomeEsposa { get; set; } = null!;
        public string NomeCampus { get; set; } = null!;
        public string NomeCurso { get; set; } = null!;
        public bool ParticipaGC { get; set; }
        public string? NomeGC { get; set; }
        public DateTime DataInscricao { get; set; }
    }
}
