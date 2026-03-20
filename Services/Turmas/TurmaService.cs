using EnsinoApp.Models.Entities;
using EnsinoApp.Repositories.Turmas;
using EnsinoApp.ViewModels.Turmas;

namespace EnsinoApp.Services.Turmas;

public class TurmaService : ITurmaService
{
    private readonly ITurmaRepository _repository;

    public TurmaService(ITurmaRepository repository)
    {
        _repository = repository;
    }

    public int ContarAtivas()
    {
        return _repository.ContarAtivas();
    }

    public Turma Create(Turma turma)
    {
        return _repository.Create(turma);
    }

    public void Delete(int id)
    {
        _repository.Delete(id);
    }

    public IEnumerable<Turma> FindAll()
    {
        return _repository.FindAll();

    }

    public async Task<IEnumerable<TurmaSelectListViewModel>> FindAllAtivasAsync(int idCurso)
    {
        var turmas = await _repository.FindAllAtivasAsync(idCurso);

        return turmas.Select(t => new TurmaSelectListViewModel
        {
            Id = t.Id,
            Descricao = $"{t.Curso.Nome} (Líderes: {GetPrimeiroNome(t.Lider.NomeMarido)} e {GetPrimeiroNome(t.Lider.NomeEsposa)})"
        });
    }

    public string GerarNomeLideres(string nome1, string nome2)
    {

        return $"{GetPrimeiroNome(nome1)} e {GetPrimeiroNome(nome2)}";
    }
    private string GetPrimeiroNome(string nomeCompleto)
    {
        if (string.IsNullOrEmpty(nomeCompleto)) return string.Empty;
        var partes = nomeCompleto.Split(' ');
        if (partes.Length >= 2)
            return $"{partes[0]}"; // Primeiro nome
        return partes[0];
    }

    public Turma? FindById(int id)
    {
        return _repository.FindById(id);
    }

    public List<TurmaResumoViewModel> ObterResumoTurmasAtivas()
    {
        return _repository.ObterTurmasAtivasResumo();
    }

    public Turma Update(Turma turma)
    {
        return _repository.Update(turma);
    }
}