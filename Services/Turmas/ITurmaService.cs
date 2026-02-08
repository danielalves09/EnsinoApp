using EnsinoApp.Models.Entities;

namespace EnsinoApp.Services.Turmas;

public interface ITurmaService
{
    IEnumerable<Turma> FindAll();
    Turma? FindById(int id);
    Turma Create(Turma turma);
    Turma Update(Turma turma);
    void Delete(int id);
}