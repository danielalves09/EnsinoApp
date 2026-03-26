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
        return await _context.Matriculas.AsNoTracking()
            .Include(m => m.Casal)
            .Include(m => m.Turma)
            .ThenInclude(t => t.Curso)
            .Include(m => m.Turma)
            .ThenInclude(t => t.Campus)
            .ToListAsync();
    }

    public async Task<Models.Entities.Matricula?> FindByIdAsync(int id)
    {
        return await _context.Matriculas.AsNoTracking()
            .Include(m => m.Casal)
            .Include(m => m.Turma)
            .ThenInclude(t => t.Curso)
            .Include(m => m.Turma)
            .ThenInclude(t => t.Campus)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Models.Entities.Matricula>> FindByTurmaAsync(int idTurma)
    {
        return await _context.Matriculas.AsNoTracking()
            .Include(m => m.Casal)
            .Include(m => m.Turma)
            .Where(m => m.IdTurma == idTurma)
            .ToListAsync();
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

    public async Task<IEnumerable<(string Curso, int Total)>> GetMatriculasPorCursoAsync()
    {
        return await _context.Matriculas
            .Include(m => m.Turma)
                .ThenInclude(t => t.Curso)
            .Where(m => m.Status == StatusMatricula.Ativa)
            .GroupBy(m => m.Turma.Curso.Nome)
            .Select(g => new { Curso = g.Key, Total = g.Count() })
            .AsNoTracking()
            .ToListAsync()
            .ContinueWith(t => t.Result.Select(x => (x.Curso, x.Total)));
    }

    public async Task<int> GetTotalRelatoriosAsync(int idMatricula)
    {
        return await _context.Relatorios.AsNoTracking()
            .Where(r => r.IdMatricula == idMatricula)
            .CountAsync();
    }
    public async Task<int> GetTotalLicoesDoCursoAsync(int idMatricula)
    {
        return await _context.Matriculas.AsNoTracking()
            .Where(m => m.Id == idMatricula)
            .Select(m => m.Turma.Curso.Licoes.Count)
            .FirstOrDefaultAsync();
    }

    public async Task AtualizarConclusaoAsync(int idMatricula)
    {
        var matricula = await _context.Matriculas.FindAsync(idMatricula);

        if (matricula == null)
            throw new Exception("Matrícula não encontrada.");

        matricula.Status = StatusMatricula.Concluída;
        matricula.DataConclusao = DateTime.Now;

        _context.Matriculas.Update(matricula);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Models.Entities.Matricula>> GetConcluidasSemCertificadoAsync()
    {
        return await _context.Matriculas
                        .Where(m =>
                                m.Status == StatusMatricula.Concluída &&
                                !m.CertificadoEmitido)
                        .Include(m => m.Casal)
                        .Include(m => m.Turma)
                            .ThenInclude(t => t.Curso)
                        .Include(m => m.Turma)
                            .ThenInclude(t => t.Lider)
                        .Include(m => m.Turma)
                            .ThenInclude(t => t.Campus)
                        .ToListAsync();
    }
    public async Task<int> CountConcluidasSemCertificadoAsync()
    {
        return await _context.Matriculas
            .Where(m => m.Status == StatusMatricula.Concluída && !m.CertificadoEmitido)
            .CountAsync();
    }

    public async Task<Models.Entities.Matricula> GetByCodigoValidacaoAsync(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            return null;

        return await _context.Matriculas.AsNoTracking()
            .Include(m => m.Casal)
            .Include(m => m.Turma)
                .ThenInclude(t => t.Curso)
            .Include(m => m.Turma)
                .ThenInclude(t => t.Lider)
            .Include(m => m.Turma)
                .ThenInclude(t => t.Campus)
            .FirstOrDefaultAsync(m => m.CodigoValidacao == codigo);
    }


}
