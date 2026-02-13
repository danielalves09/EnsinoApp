using EnsinoApp.Models.Entities;
using EnsinoApp.Services.Campus;
using EnsinoApp.Services.Cursos;
using EnsinoApp.Services.Inscricao;
using EnsinoApp.ViewModels.Inscricao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnsinoApp.Controllers;

[AllowAnonymous]
public class InscricaoController : Controller
{
    private readonly IInscricaoOnlineService _service;
    private readonly ICampusService _campusService;
    private readonly ICursoService _cursoService;

    public InscricaoController(IInscricaoOnlineService service, ICampusService campusService, ICursoService cursoService)
    {
        _service = service;
        _campusService = campusService;
        _cursoService = cursoService;
    }

    public IActionResult Index()
    {
        ViewBag.Campuses = new SelectList(_campusService.FindAll(), "Id", "Nome");
        ViewBag.Cursos = new SelectList(_cursoService.FindAll(), "Id", "Nome");
        return View(new InscricaoOnlineViewModel());
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cadastrar(InscricaoOnlineViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Campuses = new SelectList(_campusService.FindAll(), "Id", "Nome");
            ViewBag.Cursos = new SelectList(_cursoService.FindAll(), "Id", "Nome");
            return View(model);
        }

        var inscricao = new InscricaoOnline
        {
            NomeMarido = model.NomeMarido,
            NomeEsposa = model.NomeEsposa,
            TelefoneMarido = model.TelefoneMarido,
            TelefoneEsposa = model.TelefoneEsposa,
            EmailMarido = model.EmailMarido,
            EmailEsposa = model.EmailEsposa,
            Rua = model.Rua,
            Numero = model.Numero,
            Complemento = model.Complemento,
            Bairro = model.Bairro,
            Cidade = model.Cidade,
            Estado = model.Estado,
            Cep = model.Cep,
            IdCampus = model.IdCampus,
            IdCurso = model.IdCurso,
            ParticipaGC = model.ParticipaGC,
            NomeGC = model.NomeGC
        };

        var inscricaoConfirmada = await _service.CreateAsync(inscricao);


        // Redirecionar para a página de confirmação
        var confirmacaoVM = new ConfirmacaoInscricaoViewModel
        {
            NomeMarido = inscricaoConfirmada.NomeMarido,
            NomeEsposa = inscricaoConfirmada.NomeEsposa,
            NomeCampus = _campusService.FindById(inscricaoConfirmada.IdCampus)?.Nome ?? string.Empty,
            NomeCurso = _cursoService.FindById(inscricaoConfirmada.IdCurso)?.Nome ?? string.Empty,
            ParticipaGC = inscricaoConfirmada.ParticipaGC,
            NomeGC = inscricaoConfirmada.NomeGC,
            DataInscricao = inscricaoConfirmada.DataInscricao
        };


        return View("Confirmacao", confirmacaoVM);
    }

    public IActionResult Confirmacao()
    {
        return View();
    }

    [HttpGet]
    public IActionResult GetCursosPorCampus(int campusId)
    {
        var cursos = _cursoService.FindAll()
            .Where(c => c.IdCampus == campusId)
            .Select(c => new { c.Id, c.Nome })
            .ToList();

        return Json(cursos);
    }

    public async Task<IActionResult> Processar(int id)
    {
        await _service.ProcessarAsync(id);
        return RedirectToAction("Index");
    }
}
