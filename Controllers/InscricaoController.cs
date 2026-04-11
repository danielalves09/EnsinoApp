using EnsinoApp.Models.Entities;
using EnsinoApp.Services.Campus;
using EnsinoApp.Services.Cursos;
using EnsinoApp.Services.Email;
using EnsinoApp.Services.Inscricao;
using EnsinoApp.Services.Licao;
using EnsinoApp.Services.PeriodoInscricao;
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
    private readonly IPeriodoInscricaoService _periodoService;
    private readonly ILicaoService _licaoService;

    private readonly IEmailService _emailService;

    public InscricaoController(
        IInscricaoOnlineService service,
        ICampusService campusService,
        ICursoService cursoService,
        IPeriodoInscricaoService periodoService,
        ILicaoService licaoService,
        IEmailService emailService)
    {
        _service = service;
        _campusService = campusService;
        _cursoService = cursoService;
        _periodoService = periodoService;
        _licaoService = licaoService;
        _emailService = emailService;
    }

    // ── GET /Inscricao ───────────────────────────────────────────
    // Exibe a página do wizard. A verificação de períodos abertos
    // é feita pelo próprio JS ao chamar GetCampusAbertos().
    public IActionResult Index()
    {
        return View(new InscricaoOnlineViewModel());
    }

    // ── API: campus com pelo menos um período aberto ─────────────
    [HttpGet]
    public async Task<IActionResult> GetCampusAbertos()
    {
        var periodos = await _periodoService.FindTodosAbertosAsync();

        // Agrupa por campus e soma vagas restantes
        var resultado = periodos
            .GroupBy(p => new { p.IdCampus, NomeCampus = p.Campus.Nome })
            .Select(g => new
            {
                id = g.Key.IdCampus,
                nome = g.Key.NomeCampus,
                vagasRestantes = g.Sum(p => p.VagasTotal - p.VagasOcupadas)
            })
            .OrderBy(x => x.nome)
            .ToList();

        return Json(resultado);
    }

    // ── API: cursos abertos para um campus específico ────────────
    [HttpGet]
    public async Task<IActionResult> GetCursosAbertos(int campusId)
    {
        var periodos = await _periodoService.FindTodosAbertosAsync();

        var periodosNoCampus = periodos
            .Where(p => p.IdCampus == campusId)
            .ToList();

        if (!periodosNoCampus.Any())
            return Json(new List<object>());

        var cursoIds = periodosNoCampus.Select(p => p.IdCurso).Distinct().ToList();

        // Carrega cursos com suas lições
        var resultado = new List<object>();

        foreach (var periodo in periodosNoCampus)
        {
            var curso = _cursoService.FindById(periodo.IdCurso);
            if (curso == null) continue;

            // Busca lições do curso para exibir na listagem
            var licoes = await _licaoService.FindByCursoAsync(curso.Id);
            var nomesLicoes = licoes
                .OrderBy(l => l.NumeroSemana)
                .Select(l => l.Titulo)
                .ToList();

            resultado.Add(new
            {
                id = curso.Id,
                nome = curso.Nome,
                descricao = curso.Descricao,
                licoes = nomesLicoes,
                vagasRestantes = periodo.VagasTotal - periodo.VagasOcupadas
            });
        }

        return Json(resultado.OrderBy(x => ((dynamic)x).nome));
    }

    // ── POST /Inscricao/Cadastrar ────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cadastrar(InscricaoOnlineViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Devolve ao passo 3 com os dados preservados
            ViewBag.ErroValidacao = true;
            ViewBag.IdCampus = model.IdCampus;
            ViewBag.IdCurso = model.IdCurso;

            // Recupera nomes para reexibir no breadcrumb
            var campus = _campusService.FindById(model.IdCampus);
            var curso = _cursoService.FindById(model.IdCurso);
            ViewBag.NomeCampus = campus?.Nome ?? string.Empty;
            ViewBag.NomeCurso = curso?.Nome ?? string.Empty;

            return View("Index", model);
        }

        // ── Valida período e reserva vaga ────────────────────────
        try
        {
            await _periodoService.ReservarVagaAsync(model.IdCurso, model.IdCampus);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.ErroValidacao = true;
            ViewBag.IdCampus = model.IdCampus;
            ViewBag.IdCurso = model.IdCurso;

            var campus = _campusService.FindById(model.IdCampus);
            var curso = _cursoService.FindById(model.IdCurso);
            ViewBag.NomeCampus = campus?.Nome ?? string.Empty;
            ViewBag.NomeCurso = curso?.Nome ?? string.Empty;

            return View("Index", model);
        }

        // ── Cria a inscrição ─────────────────────────────────────
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

        // Se o repositório retornou null por algum motivo, usa o objeto local
        var dadosEmail = inscricaoConfirmada; //?? inscricao;

        var nomeCampus = _campusService.FindById(dadosEmail!.IdCampus)?.Nome ?? string.Empty;
        var nomeCurso = _cursoService.FindById(dadosEmail.IdCurso)?.Nome ?? string.Empty;

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

    // ── Confirmação (GET direto — mantido para compatibilidade) ──
    public IActionResult Confirmacao()
    {
        return View();
    }

    // ── Processar inscrição (admin) ──────────────────────────────
    public async Task<IActionResult> Processar(int id)
    {
        await _service.ProcessarAsync(id);
        return RedirectToAction("Index");
    }
}