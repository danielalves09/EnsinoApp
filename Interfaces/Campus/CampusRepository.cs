
using EnsinoApp.Data;
using Microsoft.EntityFrameworkCore;

namespace EnsinoApp.Interfaces.Campus;

public class CampusRepository : ICampusRepository
{

    private readonly EnsinoAppContext _context;

    public CampusRepository(EnsinoAppContext context)
    {
        _context = context;
    }

    public Models.Entities.Campus Create(Models.Entities.Campus model)
    {
        _context.Campuses.Add(model);
        _context.SaveChanges();
        return model;
    }

    public void Delete(int id)
    {
        var campus = _context.Campuses.Find(id);
        if (campus != null)
        {
            _context.Campuses.Remove(campus);
            _context.SaveChanges();
        }
    }

    public ICollection<Models.Entities.Campus> FindAll()
    {
        return _context.Campuses.AsNoTracking().ToList();
    }

    public Models.Entities.Campus? FindById(int id)
    {
        return _context.Campuses.AsNoTracking().FirstOrDefault(c => c.Id == id);
    }

    public Models.Entities.Campus Update(Models.Entities.Campus model)
    {
        _context.Campuses.Update(model);
        _context.SaveChanges();
        return model;
    }
}