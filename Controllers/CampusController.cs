using System.Text.Json;
using EnsinoApp.Models.Entities;
using EnsinoApp.Services.Campus;
using EnsinoApp.ViewModels.campus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnsinoApp.Controllers;

[Authorize]
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

    public IActionResult Adicionar()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Adicionar(AdicionarCampusViewModel dados)
    {
        var novoCampus = new Campus
        {
            Nome = dados.Nome,
            Telefone = dados.Telefone,
            Rua = dados.Rua,
            Numero = dados.Numero,
            Complemento = dados.Complemento,
            Bairro = dados.Bairro,
            Cidade = dados.Cidade,
            Estado = dados.Estado,
            Cep = dados.Cep
        };

        _campusService.Create(novoCampus);

        return RedirectToAction(nameof(Index));

    }

    public IActionResult Buscar(string filtro)
    {
        var campus = _campusService.FindAll(filtro).Select(c => new
        {
            Id = c.Id,
            Nome = c.Nome
        });

        return Json(campus);


    }

}