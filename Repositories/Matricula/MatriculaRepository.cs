using EnsinoApp.Data;
using EnsinoApp.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace EnsinoApp.Repositories.Matricula;

public class MatriculaRepository : IMatriculaRepository
{
    private readonly EnsinoAppContext _context;

    public MatriculaRepository(EnsinoAppContext context)
    {
        _context = context;
    }

    public async Task<List<Models.Entities.Matricula>> FindAllAsync()
    {
        return await _context.Matriculas
            .Include(m => m.Casal)
            .Include(m => m.Turma)
            .ThenInclude(t => t.Curso)
            .Include(m => m.Turma)
            .ThenInclude(t => t.Campus)
            .ToListAsync();
    }

    public async Task<Models.Entities.Matricula?> FindByIdAsync(int id)
    {
        return await _context.Matriculas
            .Include(m => m.Casal)
            .Include(m => m.Turma)
            .ThenInclude(t => t.Curso)
            .Include(m => m.Turma)
            .ThenInclude(t => t.Campus)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task CreateAsync(Models.Entities.Matricula matricula)
    {
        _context.Matriculas.Add(matricula);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Models.Entities.Matricula matricula)
    {
        _context.Matriculas.Update(matricula);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var matricula = await _context.Matriculas.FindAsync(id);
        if (matricula != null)
        {
            _context.Matriculas.Remove(matricula);
            await _context.SaveChangesAsync();
        }
    }

    public int ContarAtivas()
    {
        return _context.Matriculas.Count(m => m.Status == StatusMatricula.Ativa);
    }

    public bool ExisteMatriculaAtiva(int idCasal, int idTurma)
    {
        return _context.Matriculas.Any(m =>
            m.IdCasal == idCasal &&
            m.IdTurma == idTurma &&
            m.Status == StatusMatricula.Ativa);
    }

    public bool ExisteMatriculaAtivaPorCasal(int idCasal)
    {
        return _context.Matriculas.Any(m =>
            m.IdCasal == idCasal &&
            m.Status == StatusMatricula.Ativa
        );
    }


}
