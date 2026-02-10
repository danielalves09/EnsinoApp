namespace EnsinoApp.Repositories.Inscricao;

public interface IInscricaoOnlineRepository
{
    Task<List<Models.Entities.InscricaoOnline>> FindAllAsync();
    Task<Models.Entities.InscricaoOnline?> FindByIdAsync(int id);
    Task CreateAsync(Models.Entities.InscricaoOnline inscricao);
    Task UpdateAsync(Models.Entities.InscricaoOnline inscricao);
    Task DeleteAsync(int id);
}
