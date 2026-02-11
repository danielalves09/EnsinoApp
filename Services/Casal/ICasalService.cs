using EnsinoApp.ViewModels.Casal;

namespace EnsinoApp.Services.Casal;

public interface ICasalService
{
    int ContarTotal();
    List<CasalResumoViewModel> ObterResumoCasais();
}