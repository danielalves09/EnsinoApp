using EnsinoApp.Data;
using EnsinoApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

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
        return _context.Users
            .Include(u => u.Campus)
            .Include(u => u.Supervisao)
            .ToList();
    }

    public IEnumerable<Usuario> FindByCampus(int idCampus)
    {
        return _context.Users
           .Include(u => u.Campus)
           .Where(u => u.IdCampus == idCampus)
           .ToList();
    }

    public IEnumerable<Usuario> findBySupervisao(int idSupervisao)
    {
        return _context.Users
            .Include(u => u.Supervisao)
            .Where(u => u.IdSupervisao == idSupervisao)
            .ToList();
    }

    public Usuario? FindById(int id)
    {
        return _context.Users
            .Include(u => u.Campus)
            .Include(u => u.Supervisao)
            .FirstOrDefault(u => u.Id == id);
    }
}