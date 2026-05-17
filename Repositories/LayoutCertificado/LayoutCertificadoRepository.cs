using EnsinoApp.Data;
using EnsinoApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EnsinoApp.Repositories.LayoutCertificado;

public class LayoutCertificadoRepository : ILayoutCertificadoRepository
{
    private readonly EnsinoAppContext _context;

    public LayoutCertificadoRepository(EnsinoAppContext context)
    {
        _context = context;
    }

    public async Task<List<Models.Entities.LayoutCertificado>> FindAllAsync()
    {
        return await _context.LayoutsCertificado
            .AsNoTracking()
            .OrderByDescending(l => l.DataCriacao)
            .ToListAsync();
    }

    public async Task<List<Models.Entities.LayoutCertificado>> FindAllAtivosAsync()
    {
        return await _context.LayoutsCertificado
            .AsNoTracking()
            .Where(l => l.Ativo)
            .OrderBy(l => l.Nome)
            .ToListAsync();
    }

    public async Task<Models.Entities.LayoutCertificado?> FindByIdAsync(int id)
    {
        return await _context.LayoutsCertificado
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<Models.Entities.LayoutCertificado> CreateAsync(Models.Entities.LayoutCertificado layout)
    {
        _context.LayoutsCertificado.Add(layout);
        await _context.SaveChangesAsync();
        return layout;
    }

    public async Task<Models.Entities.LayoutCertificado> UpdateAsync(Models.Entities.LayoutCertificado layout)
    {
        _context.LayoutsCertificado.Update(layout);
        await _context.SaveChangesAsync();
        return layout;
    }

    public async Task DeleteAsync(int id)
    {
        var layout = await _context.LayoutsCertificado.FindAsync(id);
        if (layout is null) return;

        // Verificar se há cursos vinculados
        var cursosVinculados = await _context.Cursos.AnyAsync(c => c.IdLayoutCertificado == id);
        if (cursosVinculados)
            throw new InvalidOperationException(
                "Não é possível excluir um layout que está vinculado a cursos. Desvincule os cursos primeiro.");

        _context.LayoutsCertificado.Remove(layout);
        await _context.SaveChangesAsync();
    }
}