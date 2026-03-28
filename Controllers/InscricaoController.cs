using EnsinoApp.Models.Entities;
using EnsinoApp.Services.Campus;
using EnsinoApp.Services.Cursos;
using EnsinoApp.Services.Email;
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
    private readonly IEmailService _emailService;

    public InscricaoController(IInscricaoOnlineService service, ICampusService campusService, ICursoService cursoService, IEmailService emailService)
    {
        _service = service;
        _campusService = campusService;
        _cursoService = cursoService;
        _emailService = emailService;
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
            NomeGC = model.NomeGC,
            DataInscricao = DateTime.Now
        };

        var inscricaoSalva = await _service.CreateAsync(inscricao);

        // Se o repositório retornou null por algum motivo, usa o objeto local
        var dadosEmail = inscricaoSalva ?? inscricao;

        var nomeCampus = _campusService.FindById(dadosEmail.IdCampus)?.Nome ?? string.Empty;
        var nomeCurso = _cursoService.FindById(dadosEmail.IdCurso)?.Nome ?? string.Empty;

        var confirmacaoVM = new ConfirmacaoInscricaoViewModel
        {
            NomeMarido = dadosEmail.NomeMarido,
            NomeEsposa = dadosEmail.NomeEsposa,
            NomeCampus = nomeCampus,
            NomeCurso = nomeCurso,
            ParticipaGC = dadosEmail.ParticipaGC,
            NomeGC = dadosEmail.NomeGC,
            DataInscricao = dadosEmail.DataInscricao
        };

        // Dispara o email em background — falha não bloqueia o fluxo
        _ = _emailService.SendInscricaoConfirmadaAsync(
            dadosEmail.EmailMarido,
            dadosEmail.EmailEsposa,
            dadosEmail.NomeMarido,
            dadosEmail.NomeEsposa,
            nomeCurso,
            nomeCampus,
            dadosEmail.ParticipaGC,
            dadosEmail.NomeGC,
            dadosEmail.DataInscricao);

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
