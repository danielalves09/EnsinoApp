namespace EnsinoApp.Repositories;

public interface ICrudRepository<Model>
{

    ICollection<Model> FindAll();
    Model? FindById(int id);
    Model Create(Model model);
    Model Update(Model model);
    void Delete(int id);
}