using EnsinoApp.Data;
using EnsinoApp.Models.Entities;
using EnsinoApp.Models.Enums;
using EnsinoApp.Repositories.Casal;
using EnsinoApp.Repositories.Inscricao;
using EnsinoApp.Repositories.Matricula;
using EnsinoApp.ViewModels.Inscricao;

namespace EnsinoApp.Services.Inscricao;

public class InscricaoOnlineService : IInscricaoOnlineService
{
    private readonly IInscricaoOnlineRepository _repository;
    private readonly ICasalRepository _casalRepository;

    private readonly IMatriculaRepository _matriculaRepository;
    private readonly EnsinoAppContext _context;

    public InscricaoOnlineService(IInscricaoOnlineRepository repository, EnsinoAppContext context, ICasalRepository casalRepository, IMatriculaRepository matriculaRepository)
    {
        _repository = repository;
        _context = context;
        _casalRepository = casalRepository;
        _matriculaRepository = matriculaRepository;
    }

    public Task<List<Models.Entities.InscricaoOnline>> FindAllAsync() => _repository.FindAllAsync();
    public Task<Models.Entities.InscricaoOnline?> FindByIdAsync(int id) => _repository.FindByIdAsync(id);
    Task<InscricaoOnline?> IInscricaoOnlineService.CreateAsync(InscricaoOnline inscricao)
    {
        return _repository.CreateAsync(inscricao);
    }
    public Task<Models.Entities.InscricaoOnline> UpdateAsync(Models.Entities.InscricaoOnline inscricao) => _repository.UpdateAsync(inscricao);

    public async Task ProcessarAsync(int id)
    {
        var inscricao = await _repository.FindByIdAsync(id);
        if (inscricao == null || inscricao.Processada) return;

        // Criar Casal
        var casal = new Models.Entities.Casal
        {
            NomeConjuge1 = inscricao.NomeMarido,
            NomeConjuge2 = inscricao.NomeEsposa,
            TelefoneConjuge1 = inscricao.TelefoneMarido,
            TelefoneConjuge2 = inscricao.TelefoneEsposa,
            EmailConjuge1 = inscricao.EmailMarido,
            EmailConjuge2 = inscricao.EmailEsposa,

            Rua = inscricao.Rua,
            Numero = inscricao.Numero,
            Complemento = inscricao.Complemento,
            Bairro = inscricao.Bairro,
            Cidade = inscricao.Cidade,
            Estado = inscricao.Estado,
            Cep = inscricao.Cep,
            IdCampus = inscricao.IdCampus
        };

        _casalRepository.CreateAsync(casal).Wait();


        var matricula = new Models.Entities.Matricula
        {
            IdCasal = casal.Id,
            IdTurma = 0, // Será definida pelo admin
            DataMatricula = DateTime.Now,
            Status = StatusMatricula.Ativa,
            NomeGC = inscricao.NomeGC
        };

        _matriculaRepository.CreateAsync(matricula).Wait();

        // Marcar inscrição como processada
        inscricao.Processada = true;
        _context.Set<InscricaoOnline>().Update(inscricao);

        await _context.SaveChangesAsync();
    }

    public int ContarTotal()
    {
        return _repository.ContarTotal();
    }

    public int ContarPendentes()
    {
        return _repository.ContarPendentes();
    }

    public List<InscricaoOnlineResumoViewModel> ObterPendentesResumo()
    {
        return _repository.ObterPendentesResumo();
    }

    public async Task<IEnumerable<(string MesAno, int Total)>> GetUltimosMesesAsync(int meses)
    {
        return await _repository.GetUltimosMesesAsync(meses);
    }


}