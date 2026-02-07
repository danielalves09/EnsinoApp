namespace EnsinoApp.Services.Supervisao;

public interface ISupervisaoService
{

    ICollection<Models.Entities.Supervisao> FindAll();
    ICollection<Models.Entities.Supervisao> FindAll(string filtro);

    Models.Entities.Supervisao Create(Models.Entities.Supervisao model);
    Models.Entities.Supervisao? FindById(int id);
    Models.Entities.Supervisao Update(Models.Entities.Supervisao model);
    void Delete(int id);

}