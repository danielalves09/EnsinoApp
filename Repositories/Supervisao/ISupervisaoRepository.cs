
namespace EnsinoApp.Repositories.Supervisao;


public interface ISupervisaoRepository : ICrudRepository<Models.Entities.Supervisao>
{
    ICollection<Models.Entities.Supervisao> FindAll(string filtro);
}