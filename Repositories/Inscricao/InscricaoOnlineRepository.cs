using EnsinoApp.Data;
using EnsinoApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnsinoApp.Repositories.Inscricao;

public class InscricaoOnlineRepository : IInscricaoOnlineRepository
{
    private readonly EnsinoAppContext _context;

    public InscricaoOnlineRepository(EnsinoAppContext context)
    {
        _context = context;
    }

    public async Task<List<Models.Entities.InscricaoOnline>> FindAllAsync()
    {
        return await _context.Set<InscricaoOnline>()
            .Include(i => i.Campus)
            .Include(i => i.Curso)
            .ToListAsync();
    }

    public async Task<Models.Entities.InscricaoOnline?> FindByIdAsync(int id)
    {
        return await _context.Set<InscricaoOnline>()
            .Include(i => i.Campus)
            .Include(i => i.Curso)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task CreateAsync(Models.Entities.InscricaoOnline inscricao)
    {
        _context.Set<InscricaoOnline>().Add(inscricao);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Models.Entities.InscricaoOnline inscricao)
    {
        _context.Set<InscricaoOnline>().Update(inscricao);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var inscricao = await _context.Set<InscricaoOnline>().FindAsync(id);
        if (inscricao != null)
        {
            _context.Set<InscricaoOnline>().Remove(inscricao);
            await _context.SaveChangesAsync();
        }
    }
}
