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
        return _context.Users.AsNoTracking()
            .Include(u => u.Campus)
            .Include(u => u.Supervisao)
            .ToList();
    }

    public IEnumerable<Usuario> FindByCampus(int idCampus)
    {
        return _context.Users.AsNoTracking()
           .Include(u => u.Campus)
           .Where(u => u.IdCampus == idCampus)
           .ToList();
    }

    public IEnumerable<Usuario> findBySupervisao(int idSupervisao)
    {
        return _context.Users
            .Include(u => u.Supervisao)
            .Where(u => u.IdSupervisao == idSupervisao)
            .AsNoTracking()
            .ToList();
    }

    public Usuario? FindById(int id)
    {
        return _context.Users
            .Include(u => u.Campus)
            .Include(u => u.Supervisao)
            .AsNoTracking()
            .FirstOrDefault(u => u.Id == id);
    }

    /// <summary>
    /// Conta líderes diretamente no banco via JOIN entre AspNetUsers,
    /// AspNetUserRoles e AspNetRoles. Retorna apenas COUNT(*), sem
    /// carregar nenhuma entidade em memória.
    /// </summary>
    public Task<int> ContarLideresAsync()
    {
        return (from user in _context.Users
                join userRole in _context.UserRoles on user.Id equals userRole.UserId
                join role in _context.Roles on userRole.RoleId equals role.Id
                where role.Name == "Lider"
                select user.Id)
               .CountAsync();
    }

}