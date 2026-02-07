using EnsinoApp.Data;
using EnsinoApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnsinoApp.Repositories.Cursos;

public class CursoRepository : ICursoRepository
{
    private readonly EnsinoAppContext _context;

    public CursoRepository(EnsinoAppContext context)
    {
        _context = context;
    }

    public Curso Create(Curso model)
    {
        _context.Cursos.Add(model);
        _context.SaveChanges();

        return model;
    }

    public void Delete(int id)
    {
        var curso = _context.Cursos.Find(id);
        if (curso == null) return;

        _context.Cursos.Remove(curso);
        _context.SaveChanges();
    }

    public ICollection<Curso> FindAll()
    {
        return _context.Cursos.Include(c => c.Campus)
                          .AsNoTracking()
                          .ToList();
    }

    public Curso? FindById(int id)
    {
        return _context.Cursos.Include(c => c.Campus)
                          .FirstOrDefault(c => c.Id == id);
    }

    public Curso Update(Curso model)
    {
        _context.Cursos.Update(model);
        _context.SaveChanges();

        return model;
    }
}