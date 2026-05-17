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
        public string FontWoff2Url { get; set; } = String.Empty;
        public string FontWoffUrl { get; set; } = String.Empty;
        public string FontTtfUrl { get; set; } = String.Empty;
        public string CodigoValidacao { get; set; } = null!;
        public string QRCodeBase64 { get; set; } = null!;

        /// <summary>
        /// ID do layout dinâmico cadastrado no módulo de layouts de certificado.
        /// Quando nulo, usa o template Razor padrão (CertificadoTemplate.cshtml).
        /// </summary>
        public int? IdLayoutCertificado { get; set; }

        public string Orientacao { get; set; } = "Landscape";


    }
}
