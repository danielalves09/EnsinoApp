using EnsinoApp.Models.Entities;

namespace EnsinoApp.Repositories.LayoutCertificado;

public interface ILayoutCertificadoRepository
{
    Task<List<Models.Entities.LayoutCertificado>> FindAllAsync();
    Task<List<Models.Entities.LayoutCertificado>> FindAllAtivosAsync();
    Task<Models.Entities.LayoutCertificado?> FindByIdAsync(int id);
    Task<Models.Entities.LayoutCertificado> CreateAsync(Models.Entities.LayoutCertificado layout);
    Task<Models.Entities.LayoutCertificado> UpdateAsync(Models.Entities.LayoutCertificado layout);
    Task DeleteAsync(int id);
}