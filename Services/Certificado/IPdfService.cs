using Microsoft.EntityFrameworkCore.Infrastructure;
namespace EnsinoApp.Services.Certificado;

public interface IPdfService
{
    Task<string> GerarCertificadoAsync(Models.Entities.Matricula matricula);
}
