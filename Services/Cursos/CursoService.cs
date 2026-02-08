using EnsinoApp.Models.Entities;
using EnsinoApp.Repositories.Campus;
using EnsinoApp.Repositories.Cursos;
using EnsinoApp.ViewModels.Cursos;

namespace EnsinoApp.Services.Cursos;


public class CursoService : ICursoService
{
    private readonly ICursoRepository _repository;
    private readonly ICampusRepository _campusRepository;

    public CursoService(ICursoRepository repository, ICampusRepository campusRepository)
    {
        _repository = repository;
        _campusRepository = campusRepository;
    }

    public Curso Create(Curso model)
    {
        var cursoCreated = _repository.Create(model);
        return cursoCreated;
    }

    public void Delete(int id)
    {
        _repository.Delete(id);
    }

    public ICollection<Curso> FindAll()
    {
        return _repository.FindAll();
    }

    public ICollection<Curso> FindAll(string filtro)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Curso> FindAllDashboard()
    {
        return _repository.FindAllDashboard();
    }

    public Curso? FindById(int id)
    {
        return _repository.FindById(id);
    }

    public Curso? FindByIdDashboard(int id)
    {
        return _repository.FindByIdDashboard(id);
    }

    public Curso Update(Curso model)
    {
        var cursoUpdated = _repository.Update(model);
        return cursoUpdated;
    }
}