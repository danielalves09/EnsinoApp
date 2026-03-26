using EnsinoApp.Data;
using Microsoft.EntityFrameworkCore;

namespace EnsinoApp.Repositories.RelatorioSemanal;

public class RelatorioSemanalRepository : IRelatorioSemanalRepository
{
    private readonly EnsinoAppContext _context;

    public RelatorioSemanalRepository(EnsinoAppContext context)
    {
        _context = context;
    }

    public async Task<List<Models.Entities.Turma>> GetTurmasDoLiderAsync(int idUsuario)
    {
        return await _context.Turmas.AsNoTracking()
            .Include(t => t.Curso)
            .ThenInclude(t => t.Licoes)
            .Include(t => t.Matriculas)
            .ThenInclude(t => t.Relatorios)
            .Include(t => t.Campus)
            .Where(t => t.IdLider == idUsuario)
            .ToListAsync();
    }

    public async Task<List<Models.Entities.Matricula>> GetMatriculasPorTurmaAsync(int idTurma)
    {
        return await _context.Matriculas.AsNoTracking()
            .Include(m => m.Casal)
            .Where(m => m.IdTurma == idTurma)
            .ToListAsync();
    }

    public async Task<List<Models.Entities.RelatorioSemanal>> GetRelatoriosAsync(int idTurma)
    {
        return await _context.Relatorios.AsNoTracking()
            .Include(r => r.Matricula)
                .ThenInclude(m => m.Casal)
            .Include(r => r.Licao)
            .Where(r => r.Matricula.IdTurma == idTurma)
            .OrderByDescending(r => r.DataRegistro)
            .ToListAsync();
    }

    public async Task AddAsync(Models.Entities.RelatorioSemanal relatorio)
    {
        _context.Relatorios.Add(relatorio);
        await _context.SaveChangesAsync();
    }
}
