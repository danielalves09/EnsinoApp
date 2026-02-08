using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EnsinoApp.Services.Cursos;


public interface ICursoService
{
    ICollection<Models.Entities.Curso> FindAll();
    ICollection<Models.Entities.Curso> FindAll(string filtro);

    Models.Entities.Curso Create(Models.Entities.Curso model);
    Models.Entities.Curso? FindById(int id);
    Models.Entities.Curso Update(Models.Entities.Curso model);
    void Delete(int id);

    IEnumerable<Models.Entities.Curso> FindAllDashboard();
    Models.Entities.Curso? FindByIdDashboard(int id);

}