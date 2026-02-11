using EnsinoApp.Models.Entities;
using EnsinoApp.ViewModels.Turmas;

namespace EnsinoApp.Services.Turmas;

public interface ITurmaService
{
    IEnumerable<Turma> FindAll();
    Turma? FindById(int id);
    Turma Create(Turma turma);
    Turma Update(Turma turma);
    void Delete(int id);

    int ContarAtivas();
    List<TurmaResumoViewModel> ObterResumoTurmasAtivas();

    Task<IEnumerable<TurmaSelectListViewModel>> FindAllAtivasAsync(int idCurso);
}