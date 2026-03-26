
using EnsinoApp.Data;
using Microsoft.EntityFrameworkCore;

namespace EnsinoApp.Repositories.Licao;


public class LicaoRepository : ILicaoRepository
{

    private readonly EnsinoAppContext _context;

    public LicaoRepository(EnsinoAppContext context)
    {
        _context = context;
    }

    public async Task<Models.Entities.Licao> CreateAsync(Models.Entities.Licao licao)
    {
        _context.Licoes.Add(licao);
        await _context.SaveChangesAsync();
        return licao;
    }

    public async Task DeleteAsync(int id)
    {
        var licao = await FindByIdAsync(id);
        if (licao == null) return;

        // Evitar exclusão se tiver relatório vinculado
        if (_context.Relatorios.Any(r => r.IdLicao == id))
            throw new InvalidOperationException("Não é possível excluir lição com relatórios vinculados.");

        _context.Licoes.Remove(licao);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Models.Entities.Licao>> FindByCursoAsync(int cursoId)
    {
        return await _context.Licoes.AsNoTracking()
                .Where(l => l.IdCurso == cursoId)
                .OrderBy(l => l.NumeroSemana)
                .ToListAsync();

    }

    public async Task<Models.Entities.Licao> FindByIdAsync(int id)
    {
        return await _context.Licoes.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<ICollection<Models.Entities.Licao>> GetByCursoAsync(int cursoId)
    {
        return await _context.Licoes.AsNoTracking()
               .Where(l => l.IdCurso == cursoId)
               .OrderBy(l => l.NumeroSemana)
               .ToListAsync();
    }

    public async Task<Models.Entities.Licao> UpdateAsync(Models.Entities.Licao licao)
    {
        _context.Licoes.Update(licao);
        await _context.SaveChangesAsync();
        return licao;
    }
}