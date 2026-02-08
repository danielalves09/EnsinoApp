using EnsinoApp.Data;
using EnsinoApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnsinoApp.Repositories.Turmas;

public class TurmaRepository : ITurmaRepository
{

    private readonly EnsinoAppContext _context;

    public TurmaRepository(EnsinoAppContext context)
    {
        _context = context;
    }

    public ICollection<Turma> FindAll()
    {
        return _context.Turmas
             .Include(t => t.Curso)
             .Include(t => t.Campus)
             .Include(t => t.Lider)
             .AsNoTracking()
             .ToList();
    }

    public Turma? FindById(int id)
    {
        return _context.Turmas
            .Include(t => t.Curso)
            .Include(t => t.Campus)
            .Include(t => t.Lider)
            .Include(t => t.Matriculas)
            .ThenInclude(m => m.Casal)
            .FirstOrDefault(t => t.Id == id);
    }

    public Turma Create(Turma model)
    {
        _context.Turmas.Add(model);
        _context.SaveChanges();
        return model;
    }

    public Turma Update(Turma model)
    {
        _context.Turmas.Update(model);
        _context.SaveChanges();

        return model;
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }




}