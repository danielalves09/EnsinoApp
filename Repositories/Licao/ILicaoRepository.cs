namespace EnsinoApp.Repositories.Licao;

public interface ILicaoRepository
{

    Task<IEnumerable<Models.Entities.Licao>> FindByCursoAsync(int cursoId);
    Task<ICollection<Models.Entities.Licao>> GetByCursoAsync(int cursoId);
    Task<Models.Entities.Licao> FindByIdAsync(int id);
    Task<Models.Entities.Licao> CreateAsync(Models.Entities.Licao licao);
    Task<Models.Entities.Licao> UpdateAsync(Models.Entities.Licao licao);
    Task DeleteAsync(int id);
}