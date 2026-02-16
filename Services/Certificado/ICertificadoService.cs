using EnsinoApp.ViewModels.Certificado;

namespace EnsinoApp.Services.Certificado;

public interface ICertificadoService
{
    Task<List<string>> GerarCertificadosAsync();
    Task<byte[]> GerarCertificadoPdfAsync(CertificadoViewModel model);

    string GerarNomeCasal(string nome1, string nome2);

    string GerarCodigoValidacao();
}