using System.ComponentModel.DataAnnotations;

namespace EnsinoApp.ViewModels.Licao
{
    public class LicaoViewModel
    {
        public int Id { get; set; }
        public int IdCurso { get; set; }

        [Required]
        [Range(1, 100)]
        public int Numero { get; set; }

        [Required]
        public string Titulo { get; set; } = null!;

        [StringLength(500)]
        public string? Descricao { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? DataAula { get; set; }
    }
}
