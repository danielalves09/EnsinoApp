using EnsinoApp.Data;
using EnsinoApp.Models.Entities;
using EnsinoApp.ViewModels.Inscricao;
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
        return await _context.Set<InscricaoOnline>().AsNoTracking()
            .Include(i => i.Campus)
            .Include(i => i.Curso)
            .ToListAsync();
    }

    public async Task<Models.Entities.InscricaoOnline?> FindByIdAsync(int id)
    {
        return await _context.Set<InscricaoOnline>().AsNoTracking()
            .Include(i => i.Campus)
            .Include(i => i.Curso)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Models.Entities.InscricaoOnline?> CreateAsync(Models.Entities.InscricaoOnline inscricao)
    {
        _context.Set<InscricaoOnline>().Add(inscricao);
        await _context.SaveChangesAsync();

        return inscricao;
    }

    public async Task<InscricaoOnline> UpdateAsync(InscricaoOnline inscricao)
    {
        _context.Set<InscricaoOnline>().Update(inscricao);
        await _context.SaveChangesAsync();

        return inscricao;
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

    public int ContarTotal()
    {
        return _context.InscricoesOnline.Count();
    }

    public int ContarPendentes()
    {
        return _context.InscricoesOnline.Count(i => !i.Processada);
    }

    public List<InscricaoOnlineResumoViewModel> ObterPendentesResumo()
    {
        return _context.InscricoesOnline
            .Where(i => !i.Processada)
            .Select(i => new InscricaoOnlineResumoViewModel
            {
                Id = i.Id,
                NomeCasal = $"{i.NomeMarido} e {i.NomeEsposa}",
                Curso = i.Curso.Nome,
                IdCurso = i.IdCurso,
                Campus = i.Campus.Nome,
                IdCampus = i.IdCampus,
                DataInscricao = i.DataInscricao
            })
            .ToList();
    }

    public async Task<IEnumerable<(string MesAno, int Total)>> GetUltimosMesesAsync(int meses)
    {
        var dataLimite = DateTime.Now.AddMonths(-meses + 1);


        var agrupados = await _context.InscricoesOnline
            .Where(i => i.DataInscricao >= dataLimite)
            .GroupBy(i => new { i.DataInscricao.Year, i.DataInscricao.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Count() })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .AsNoTracking()
            .ToListAsync();

        var resultados = agrupados
            .Select(x => (MesAno: x.Month.ToString("D2") + "/" + x.Year, x.Total))
            .ToList();

        return resultados;
    }




}
