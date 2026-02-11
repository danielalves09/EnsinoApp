using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EnsinoApp.Repositories.Casal;


public interface ICasalRepository : ICrudRepository<Models.Entities.Casal>
{

    int ContarTotal();
    List<Models.Entities.Casal> ObterTodos();
}