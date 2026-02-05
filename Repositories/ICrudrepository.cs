namespace EnsinoApp.Repositories;

public interface ICrudRepository<Model>
{

    ICollection<Model> FindAll();
    Model Create(Model model);
    Model? FindById(int id);
    Model Update(Model model);
    void Delete(int id);
}