using EnsinoApp.Data;
using EnsinoApp.Models.Entities;
using EnsinoApp.Models.Enums;
using EnsinoApp.ViewModels.Turmas;
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
            .ThenInclude(t => t.Licoes)
            .Include(t => t.Campus)
            .Include(t => t.Lider)
            .Include(t => t.Matriculas)
            .ThenInclude(m => m.Casal)
            .Include(t => t.Matriculas)
            .ThenInclude(t => t.Relatorios)
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

    public int ContarAtivas()
    {
        return _context.Turmas.Count(t => t.Status == StatusTurma.EmAndamento);
    }

    public List<TurmaResumoViewModel> ObterTurmasAtivasResumo()
    {
        return _context.Turmas
            .Where(t => t.Status == StatusTurma.EmAndamento)
            .Select(t => new TurmaResumoViewModel
            {
                Id = t.Id,
                Curso = t.Curso.Nome,
                Campus = t.Campus.Nome,
                DataInicio = t.DataInicio
            })
            .ToList();
    }

    public async Task<IEnumerable<Turma>> FindAllAtivasAsync(int idCurso)
    {
        return await _context.Turmas
                .Include(t => t.Curso)
                .Include(t => t.Lider)
                .Where(t => t.IdCurso == idCurso && t.Status == Models.Enums.StatusTurma.Acomecar)
                .ToListAsync();
    }
}