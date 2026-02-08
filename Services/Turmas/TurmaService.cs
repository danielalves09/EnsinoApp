using EnsinoApp.Models.Entities;
using EnsinoApp.Repositories.Turmas;

namespace EnsinoApp.Services.Turmas;

public class TurmaService : ITurmaService
{
    private readonly ITurmaRepository _repository;

    public TurmaService(ITurmaRepository repository)
    {
        _repository = repository;
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

    public Turma? FindById(int id)
    {
        return _repository.FindById(id);
    }

    public Turma Update(Turma turma)
    {
        return _repository.Update(turma);
    }
}