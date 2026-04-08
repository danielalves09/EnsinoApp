using EnsinoApp.Data;
using Microsoft.EntityFrameworkCore;

namespace EnsinoApp.Repositories.PeriodoInscricao;

public class PeriodoInscricaoRepository : IPeriodoInscricaoRepository
{
    private readonly EnsinoAppContext _context;

    public PeriodoInscricaoRepository(EnsinoAppContext context)
    {
        _context = context;
    }

    public async Task<List<Models.Entities.PeriodoInscricao>> FindAllAsync()
    {
        return await _context.PeriodosInscricao
            .AsNoTracking()
            .Include(p => p.Curso)
            .Include(p => p.Campus)
            .OrderByDescending(p => p.DataAbertura)
            .ToListAsync();
    }

    public async Task<List<Models.Entities.PeriodoInscricao>> FindByCursoAsync(int idCurso)
    {
        return await _context.PeriodosInscricao
            .AsNoTracking()
            .Include(p => p.Campus)
            .Include(p => p.Curso)
            .Where(p => p.IdCurso == idCurso)
            .OrderByDescending(p => p.DataAbertura)
            .ToListAsync();
    }

    public async Task<Models.Entities.PeriodoInscricao?> FindByIdAsync(int id)
    {
        return await _context.PeriodosInscricao
            .AsNoTracking()
            .Include(p => p.Curso)
            .Include(p => p.Campus)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Models.Entities.PeriodoInscricao?> FindAtivoAsync(int idCurso, int idCampus)
    {
        var agora = DateTime.Now;
        return await _context.PeriodosInscricao
            .AsNoTracking()
            .Include(p => p.Curso)
            .Include(p => p.Campus)
            .FirstOrDefaultAsync(p =>
                p.IdCurso == idCurso &&
                p.IdCampus == idCampus &&
                p.Ativo &&
                p.DataAbertura <= agora &&
                p.DataEncerramento >= agora &&
                p.VagasOcupadas < p.VagasTotal);
    }

    public async Task<List<Models.Entities.PeriodoInscricao>> FindTodosAbertosAsync()
    {
        var agora = DateTime.Now;
        return await _context.PeriodosInscricao
            .AsNoTracking()
            .Include(p => p.Curso)
            .Include(p => p.Campus)
            .Where(p =>
                p.Ativo &&
                p.DataAbertura <= agora &&
                p.DataEncerramento >= agora &&
                p.VagasOcupadas < p.VagasTotal)
            .ToListAsync();
    }

    public async Task<Models.Entities.PeriodoInscricao> CreateAsync(Models.Entities.PeriodoInscricao periodo)
    {
        _context.PeriodosInscricao.Add(periodo);
        await _context.SaveChangesAsync();
        return periodo;
    }

    public async Task<Models.Entities.PeriodoInscricao> UpdateAsync(Models.Entities.PeriodoInscricao periodo)
    {
        _context.PeriodosInscricao.Update(periodo);
        await _context.SaveChangesAsync();
        return periodo;
    }

    public async Task DeleteAsync(int id)
    {
        var periodo = await _context.PeriodosInscricao.FindAsync(id);
        if (periodo is null) return;
        _context.PeriodosInscricao.Remove(periodo);
        await _context.SaveChangesAsync();
    }

    public async Task IncrementarVagaAsync(int id)
    {
        // UPDATE direto, evita race condition com Load+Save
        await _context.PeriodosInscricao
            .Where(p => p.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(
                p => p.VagasOcupadas,
                p => p.VagasOcupadas + 1));
    }
}