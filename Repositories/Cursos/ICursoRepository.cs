using EnsinoApp.Models.Entities;

namespace EnsinoApp.Repositories.Cursos;

public interface ICursoRepository : ICrudRepository<Models.Entities.Curso>
{
    IEnumerable<Curso> FindAllDashboard();
    Curso? FindByIdDashboard(int id);


}