

using EnsinoApp.Data;
using EnsinoApp.Models.Enums;
using EnsinoApp.ViewModels.Casal;
using Microsoft.EntityFrameworkCore;

namespace EnsinoApp.Repositories.Casal;

public class CasalRepository : ICasalRepository
{
    private readonly EnsinoAppContext _context;

    public CasalRepository(EnsinoAppContext context)
    {
        _context = context;
    }

    public async Task<List<Models.Entities.Casal>> FindAllAsync()
    {
        return await _context.Casais
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Models.Entities.Casal?> FindByIdAsync(int id)
    {
        return await _context.Casais.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Models.Entities.Casal> CreateAsync(Models.Entities.Casal casal)
    {
        _context.Casais.Add(casal);
        await _context.SaveChangesAsync();

        return casal;
    }

    public async Task UpdateAsync(Models.Entities.Casal casal)
    {
        _context.Casais.Update(casal);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var casal = await _context.Casais.FindAsync(id);
        if (casal != null)
        {
            _context.Casais.Remove(casal);
            await _context.SaveChangesAsync();
        }
    }

    public int ContarTotal()
    {
        return _context.Casais.Count();
    }

    public List<Models.Entities.Casal> ObterTodos()
    {
        return _context.Casais.AsNoTracking()
                .OrderBy(c => c.NomeConjuge1)
                .ToList();
    }

    public async Task<IEnumerable<(string Campus, int Total)>> GetCasaisPorCampusAsync()
    {
        return await _context.Casais.AsNoTracking()
            .Include(c => c.Campus)
            .GroupBy(c => c.Campus.Nome)
            .Select(g => new { Campus = g.Key, Total = g.Count() })
            .ToListAsync()
            .ContinueWith(t => t.Result.Select(x => (x.Campus, x.Total)));
    }

    public List<CasalResumoViewModel> ObterResumoCasais()
    {
        return _context.Casais
            .AsNoTracking()
            .OrderBy(c => c.NomeConjuge1)
            .Select(c => new CasalResumoViewModel
            {
                Id = c.Id,
                NomeCasal = c.NomeConjuge1 + " e " + c.NomeConjuge2,
                PossuiMatriculaAtiva = c.Matriculas
                    .Any(m => m.Status == StatusMatricula.Ativa)
            })
            .ToList();
    }

    public async Task<Models.Entities.Casal?> FindByIdComMatriculasAsync(int id)
    {
        return await _context.Casais
            .Include(c => c.Campus)
            .Include(c => c.Matriculas)
                .ThenInclude(m => m.Turma)
                    .ThenInclude(t => t.Curso)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

}