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
        return await _context.Turmas
            .Include(t => t.Curso)
            .Where(t => t.IdLider == idUsuario)
            .ToListAsync();
    }

    public async Task<List<Models.Entities.Matricula>> GetMatriculasPorTurmaAsync(int idTurma)
    {
        return await _context.Matriculas
            .Include(m => m.Casal)
            .Where(m => m.IdTurma == idTurma)
            .ToListAsync();
    }

    public async Task<List<Models.Entities.RelatorioSemanal>> GetRelatoriosAsync(int idTurma)
    {
        return await _context.Relatorios
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
