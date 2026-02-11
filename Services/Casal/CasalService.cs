
using EnsinoApp.Repositories.Casal;
using EnsinoApp.Repositories.Matricula;
using EnsinoApp.ViewModels.Casal;

namespace EnsinoApp.Services.Casal;

public class CasalService : ICasalService
{

    private readonly ICasalRepository _casalRepository;
    private readonly IMatriculaRepository _matriculaRepository;

    public CasalService(
        ICasalRepository casalRepository,
        IMatriculaRepository matriculaRepository)
    {
        _casalRepository = casalRepository;
        _matriculaRepository = matriculaRepository;
    }
    public int ContarTotal()
    {
        return _casalRepository.ContarTotal();
    }

    public List<CasalResumoViewModel> ObterResumoCasais()
    {
        var casais = _casalRepository.ObterTodos();

        return casais.Select(c => new CasalResumoViewModel
        {
            Id = c.Id,
            NomeCasal = $"{c.NomeConjuge1} e {c.NomeConjuge2}",
            PossuiMatriculaAtiva = _matriculaRepository.ExisteMatriculaAtivaPorCasal(c.Id)
        }).ToList();
    }
}