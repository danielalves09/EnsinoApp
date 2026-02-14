namespace EnsinoApp.Services.Certificado;

public interface ICertificadoService
{
    Task<List<string>> GerarCertificadosAsync();
    Task<byte[]> GerarPdfCertificadoAsync(Models.Entities.Matricula matricula);
}