using EnsinoApp.Models.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace EnsinoApp.Repositories.Campus;

public interface ICampusRepository : ICrudRepository<Models.Entities.Campus>
{
    ICollection<Models.Entities.Campus> FindAll(string filtro);
    Task<int> ContarTotal();
}