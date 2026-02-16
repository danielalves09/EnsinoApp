namespace EnsinoApp.ViewModels.Certificado
{
    public class CertificadoViewModel
    {
        public string NomeCasal { get; set; } = null!;
        public string NomeCurso { get; set; } = null!;
        public DateTime DataConclusao { get; set; }
        public string NomeLider { get; set; } = null!;
        public string NomeCampus { get; set; } = null!;
        public string LogoUrl { get; set; } = String.Empty;
        public string FundoUrl { get; set; } = String.Empty;
        public string CodigoValidacao { get; set; } = null!;
        public string QRCodeBase64 { get; set; } = null!;
    }
}
