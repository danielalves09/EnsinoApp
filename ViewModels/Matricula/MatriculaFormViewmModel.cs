using EnsinoApp.ViewModels.Turmas;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EnsinoApp.ViewModels.Matricula
{
    public class MatriculaFormViewModel
    {
        public int IdInscricao { get; set; }
        public int? IdCasal { get; set; }

        [DisplayName("Turma")]
        public int IdTurma { get; set; }
        public string NomeCasal { get; set; } = string.Empty;
        public string NomeCampus { get; set; } = string.Empty;
        public string? NomeGC { get; set; }

        public int IdCurso { get; set; } // necessário para recarregar turmas em caso de erro
        public List<TurmaViewModel> Turmas { get; set; } = new List<TurmaViewModel>();
        public SelectList? SelectTurmas { get; set; } = null!;
    }
}
