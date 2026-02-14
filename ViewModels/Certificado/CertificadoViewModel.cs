namespace EnsinoApp.ViewModels.Certificado
{
    public class CertificadoViewModel
    {
        public string NomeCasal { get; set; } = null!;
        public string NomeCurso { get; set; } = null!;
        public DateTime DataConclusao { get; set; }
        public string NomeLider { get; set; } = null!;
        public string NomeCampus { get; set; } = null!;
        public string LogoUrl { get; set; } = "/images/logo.png";
        public string FundoUrl { get; set; } = "/images/fundo-textura.jpg"; // fundo decorativo
    }
}
