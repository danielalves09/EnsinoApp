using EnsinoApp.Models.Entities;

namespace EnsinoApp.Repositories.Usuarios;

public interface IUsuariosRepository
{
    Usuario? FindById(int id);
    IEnumerable<Usuario> FindAll();
    IEnumerable<Usuario> FindByCampus(int idCampus);
    IEnumerable<Usuario> findBySupervisao(int idSupervisao);

    Task<int> ContarLideresAsync();

}