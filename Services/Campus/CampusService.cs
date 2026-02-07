using EnsinoApp.Repositories.Campus;
namespace EnsinoApp.Services.Campus;


public class CampusService : ICampusService
{

    private readonly ICampusRepository _campusRepository;

    public CampusService(ICampusRepository campusRepository)
    {
        _campusRepository = campusRepository;
    }

    public Models.Entities.Campus Create(Models.Entities.Campus model)
    {
        var campusCreate = _campusRepository.Create(model);
        return campusCreate;
    }

    public void Delete(int id)
    {
        _campusRepository.Delete(id);
    }

    public ICollection<Models.Entities.Campus> FindAll()
    {
        return _campusRepository.FindAll();
    }

    public ICollection<Models.Entities.Campus> FindAll(string filtro)
    {
        var campusEncontrados = _campusRepository.FindAll(filtro);

        return campusEncontrados;

    }

    public Models.Entities.Campus? FindById(int id)
    {
        var campusFind = _campusRepository.FindById(id);

        return campusFind;
    }

    public Models.Entities.Campus Update(Models.Entities.Campus model)
    {
        var campusUpdate = _campusRepository.Update(model);

        return campusUpdate;
    }
}