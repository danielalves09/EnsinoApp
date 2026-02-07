
using EnsinoApp.Data;
using Microsoft.EntityFrameworkCore;

namespace EnsinoApp.Repositories.Supervisao;

public class SupervisaoRepository : ISupervisaoRepository
{
    private readonly EnsinoAppContext _context;

    public SupervisaoRepository(EnsinoAppContext context)
    {
        _context = context;
    }

    public Models.Entities.Supervisao Create(Models.Entities.Supervisao model)
    {
        _context.Supervisoes.Add(model);
        _context.SaveChanges();
        return model;
    }

    public void Delete(int id)
    {
        var Supervisao = _context.Supervisoes.Find(id);
        if (Supervisao != null)
        {
            _context.Supervisoes.Remove(Supervisao);
            _context.SaveChanges();
        }
    }

    public ICollection<Models.Entities.Supervisao> FindAll()
    {
        return _context.Supervisoes.Include(s => s.Campus).AsNoTracking().ToList();
    }

    public Models.Entities.Supervisao? FindById(int id)
    {
        return _context.Supervisoes.AsNoTracking().FirstOrDefault(c => c.Id == id);
    }

    public Models.Entities.Supervisao Update(Models.Entities.Supervisao model)
    {
        _context.Supervisoes.Update(model);
        _context.SaveChanges();
        return model;
    }
}