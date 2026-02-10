using EnsinoApp.Data;
using EnsinoApp.Models.Entities;
using EnsinoApp.Models.Enums;
using EnsinoApp.Repositories.Inscricao;

namespace EnsinoApp.Services.Inscricao;

public class InscricaoOnlineService : IInscricaoOnlineService
{
    private readonly IInscricaoOnlineRepository _repository;
    private readonly EnsinoAppContext _context;

    public InscricaoOnlineService(IInscricaoOnlineRepository repository, EnsinoAppContext context)
    {
        _repository = repository;
        _context = context;
    }

    public Task<List<Models.Entities.InscricaoOnline>> GetAllAsync() => _repository.FindAllAsync();
    public Task<Models.Entities.InscricaoOnline?> GetByIdAsync(int id) => _repository.FindByIdAsync(id);
    public Task CreateAsync(Models.Entities.InscricaoOnline inscricao) => _repository.CreateAsync(inscricao);

    public async Task ProcessarAsync(int id)
    {
        var inscricao = await _repository.FindByIdAsync(id);
        if (inscricao == null || inscricao.Processada) return;

        // Criar Casal
        var casal = new Casal
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
        _context.Casais.Add(casal);
        await _context.SaveChangesAsync();


        var matricula = new Models.Entities.Matricula
        {
            IdCasal = casal.Id,
            IdTurma = 0, // Será definida pelo admin
            DataMatricula = DateTime.Now,
            Status = StatusMatricula.Ativa,
            NomeGC = inscricao.NomeGC
        };
        _context.Matriculas.Add(matricula);

        // Marcar inscrição como processada
        inscricao.Processada = true;
        _context.Set<InscricaoOnline>().Update(inscricao);

        await _context.SaveChangesAsync();
    }
}