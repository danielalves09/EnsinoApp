

using EnsinoApp.Data;

namespace EnsinoApp.Repositories.Casal;

public class CasalRepository : ICasalRepository
{
    private readonly EnsinoAppContext _context;

    public CasalRepository(EnsinoAppContext context)
    {
        _context = context;
    }
    public int ContarTotal()
    {
        return _context.Casais.Count();
    }

    public Models.Entities.Casal Create(Models.Entities.Casal model)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public ICollection<Models.Entities.Casal> FindAll()
    {
        throw new NotImplementedException();
    }

    public Models.Entities.Casal? FindById(int id)
    {
        throw new NotImplementedException();
    }

    public List<Models.Entities.Casal> ObterTodos()
    {
        return _context.Casais
                .OrderBy(c => c.NomeConjuge1)
                .ToList();
    }

    public Models.Entities.Casal Update(Models.Entities.Casal model)
    {
        throw new NotImplementedException();
    }
}