using EnsinoApp.Models.Entities;
using EnsinoApp.ViewModels.Turmas;

namespace EnsinoApp.Repositories.Turmas;


public interface ITurmaRepository : ICrudRepository<Turma>
{
    int ContarAtivas();
    List<TurmaResumoViewModel> ObterTurmasAtivasResumo();

}