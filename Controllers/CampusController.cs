using EnsinoApp.Services.Campus;
using EnsinoApp.ViewModels.campus;
using Microsoft.AspNetCore.Mvc;

namespace EnsinoApp.Controllers;

public class CampusController : Controller
{

    private readonly ICampusService _campusService;
    private const int TAMANHO_PAGINA = 10;
    public CampusController(ICampusService campusService)
    {
        _campusService = campusService;
    }

    public IActionResult Index(string filtro, int pagina = 1)
    {
        var campus = _campusService.FindAll().Select(c => new ListarCamposViewModel
        {
            Id = c.Id,
            Nome = c.Nome,
            Telefone = c.Telefone

        });


        ViewBag.Filtro = filtro;
        ViewBag.NumeroPagina = pagina;
        ViewBag.TotalPaginas = Math.Ceiling((decimal)campus.Count() / TAMANHO_PAGINA);

        return View(campus.Skip((pagina - 1) * TAMANHO_PAGINA).Take(TAMANHO_PAGINA));

    }


}