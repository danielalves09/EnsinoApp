using EnsinoApp.Models.Entities;
using EnsinoApp.Services.Campus;
using EnsinoApp.Services.Supervisao;
using EnsinoApp.ViewModels.Supervisao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnsinoApp.Controllers;

[Authorize]
public class SupervisaoController : Controller
{
    private readonly ISupervisaoService _supervisaoService;
    private const int TAMANHO_PAGINA = 10;
    public SupervisaoController(ISupervisaoService supervisaoService)
    {
        _supervisaoService = supervisaoService;
    }

    public IActionResult Index(string filtro, int pagina = 1)
    {
        var campus = _supervisaoService.FindAll().Select(c => new ListarSupervisaoViewModel
        {
            Id = c.Id,
            Nome = c.Nome,
            NomeCampus = c.Campus.Nome
        });


        ViewBag.Filtro = filtro;
        ViewBag.NumeroPagina = pagina;
        ViewBag.TotalPaginas = Math.Ceiling((decimal)campus.Count() / TAMANHO_PAGINA);

        return View(campus.Skip((pagina - 1) * TAMANHO_PAGINA).Take(TAMANHO_PAGINA));

    }

    public IActionResult Adicionar()
    {
        return View();

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Adicionar(AdicionarSupervisaoViewModel dados)
    {
        var supervisaoNova = new Supervisao
        {
            Nome = dados.Nome,
            IdCampus = dados.IdCampus
        };

        _supervisaoService.Create(supervisaoNova);

        return RedirectToAction(nameof(Index));

    }

    public IActionResult Buscar(string filtro)
    {
        var supervisao = _supervisaoService.FindAll(filtro).Select(c => new
        {
            Id = c.Id,
            Nome = c.Nome
        });

        return Json(supervisao);


    }
}