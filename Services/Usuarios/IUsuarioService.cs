using EnsinoApp.Models.Entities;

namespace EnsinoApp.Services.Usuarios;

public interface IUsuariosService
{

    Usuario? FindById(int id);
    IEnumerable<Usuario> FindAll();
    IEnumerable<Usuario> FindByCampus(int idCampus);
    IEnumerable<Usuario> findBySupervisao(int idSupervisao);
    public string GetNomeReduzido(string nome1, string nome2);

}