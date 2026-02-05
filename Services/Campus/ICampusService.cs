using EnsinoApp.Models.Entities;

namespace EnsinoApp.Services.Campus;

public interface ICampusService
{
    ICollection<Models.Entities.Campus> FindAll();
    Models.Entities.Campus Create(Models.Entities.Campus model);
    Models.Entities.Campus? FindById(int id);
    Models.Entities.Campus Update(Models.Entities.Campus model);
    void Delete(int id);

}