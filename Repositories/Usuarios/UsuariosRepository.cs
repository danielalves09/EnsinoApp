using EnsinoApp.Data;
using EnsinoApp.Models.Entities;

namespace EnsinoApp.Repositories.Usuarios;


public class UsuariosRepository : IUsuariosRepository
{
    private readonly EnsinoAppContext _context;

    public UsuariosRepository(EnsinoAppContext context)
    {
        _context = context;
    }

    public IEnumerable<Usuario> FindAll()
    {
        return _context.Users.ToList();
    }

    public IEnumerable<Usuario> FindByCampus(int idCampus)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Usuario> findBySupervisao(int idSupervisao)
    {
        throw new NotImplementedException();
    }

    public Usuario? FindById(int id)
    {
        return _context.Users
            .FirstOrDefault(u => u.Id == id);
    }
}