using EnsinoApp.Data;
using EnsinoApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnsinoApp.Repositories.Agenda;

public class AgendaRepository : IAgendaRepository
{
    private readonly EnsinoAppContext _context;

    public AgendaRepository(EnsinoAppContext context)
    {
        _context = context;
    }

    public async Task<List<AgendaLicao>> GetByTurmaAsync(int idTurma)
    {
        return await _context.Set<AgendaLicao>()
            .AsNoTracking()
            .Include(a => a.Licao)
            .Where(a => a.IdTurma == idTurma)
            .OrderBy(a => a.DataAula)
            .ToListAsync();
    }

    public async Task<AgendaLicao?> FindByIdAsync(int id)
    {
        return await _context.Set<AgendaLicao>()
            .Include(a => a.Licao)
            .Include(a => a.Turma)
                .ThenInclude(t => t.Matriculas)
                    .ThenInclude(m => m.Casal)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    /// <summary>
    /// Retorna todos os itens de agenda cuja DataAula seja igual a <paramref name="dataAlvo"/>
    /// e o lembrete ainda não tenha sido enviado.
    /// Inclui os casais matriculados para que o serviço de email possa iterar sobre eles.
    /// </summary>
    public async Task<List<AgendaLicao>> GetPendentesLembreteAsync(DateTime dataAlvo)
    {
        var dataAlvoDate = dataAlvo.Date;

        return await _context.Set<AgendaLicao>()
            .Include(a => a.Licao)
            .Include(a => a.Turma)
                .ThenInclude(t => t.Lider)
            .Include(a => a.Turma)
                .ThenInclude(t => t.Curso)
            .Include(a => a.Turma)
                .ThenInclude(t => t.Campus)
            .Include(a => a.Turma)
                .ThenInclude(t => t.Matriculas)
                    .ThenInclude(m => m.Casal)
            .Where(a => a.DataAula.Date == dataAlvoDate && !a.LembreteEnviado)
            .ToListAsync();
    }

    public async Task CreateRangeAsync(IEnumerable<AgendaLicao> agenda)
    {
        await _context.Set<AgendaLicao>().AddRangeAsync(agenda);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AgendaLicao agenda)
    {
        _context.Set<AgendaLicao>().Update(agenda);
        await _context.SaveChangesAsync();
    }

    public async Task MarcarLembreteEnviadoAsync(int idAgenda)
    {
        var agenda = await _context.Set<AgendaLicao>().FindAsync(idAgenda);
        if (agenda is null) return;

        agenda.LembreteEnviado = true;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByTurmaAsync(int idTurma)
    {
        var itens = await _context.Set<AgendaLicao>()
            .Where(a => a.IdTurma == idTurma)
            .ToListAsync();

        _context.Set<AgendaLicao>().RemoveRange(itens);
        await _context.SaveChangesAsync();
    }
}