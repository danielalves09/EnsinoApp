namespace EnsinoApp.Services.Inscricao;

public interface IInscricaoOnlineService
{
    Task<List<Models.Entities.InscricaoOnline>> GetAllAsync();
    Task<Models.Entities.InscricaoOnline?> GetByIdAsync(int id);
    Task CreateAsync(Models.Entities.InscricaoOnline inscricao);
    Task ProcessarAsync(int id);
}