using System.Security.Claims;
using EnsinoApp.Models.Entities;
using EnsinoApp.Models.Enums;
using EnsinoApp.Services.Licao;
using EnsinoApp.Services.Lider;
using EnsinoApp.Services.Matricula;
using EnsinoApp.Services.Turmas;
using EnsinoApp.ViewModels.Lider;
using EnsinoApp.ViewModels.Relatorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnsinoApp.Controllers;


//[Area("Lider")]
//[Authorize(Roles = "Lider")]
[Authorize]
public class LiderController : Controller
{
    private readonly ILiderService _service;
    private readonly ILicaoService _licaoService;
    private readonly ITurmaService _turmaService;
    private readonly IMatriculaService _matriculaService;


    public LiderController(ILiderService service, ILicaoService licaoService, ITurmaService turmaService, IMatriculaService matriculaService)
    {
        _service = service;
        _licaoService = licaoService;
        _turmaService = turmaService;
        _matriculaService = matriculaService;
    }

    public async Task<IActionResult> Index()
    {
        var idUsuarioClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(idUsuarioClaim))
            return Unauthorized();

        int idUsuario = int.Parse(idUsuarioClaim);

        var turmas = await _service.ObterTurmasAsync(idUsuario);

        return View(turmas.Select(t => new TurmaLiderViewModel
        {
            Id = t.Id,
            Curso = t.Curso.Nome
        }));
    }

    public async Task<IActionResult> Turma(int id)
    {
        var matriculas = await _service.ObterMatriculasAsync(id);
        return View(matriculas);
    }

    public async Task<IActionResult> Relatorios(int idTurma)
    {
        var relatorios = await _service.ObterRelatoriosAsync(idTurma);
        return View(relatorios);
    }

    public async Task<IActionResult> CriarRelatorio(int idTurma)
    {
        var turma = _turmaService.FindById(idTurma);

        if (turma == null)
            return NotFound();

        var licoes = await _licaoService.FindByCursoAsync(turma.IdCurso);
        var matriculas = await _matriculaService.FindByTurmaAsync(idTurma);

        var vm = new RelatorioCreateViewModel
        {
            IdTurma = idTurma,
            IdCurso = turma.IdCurso,
            DataLicao = DateTime.Today,

            Licoes = licoes.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = $"{l.NumeroSemana}. {l.Titulo}"
            }).ToList(),

            Casais = matriculas.Select(m => new RelatorioCasalItemViewModel
            {
                IdMatricula = m.Id,
                NomeCasal = $"{m.Casal.NomeConjuge1} e {m.Casal.NomeConjuge2}",
                Presenca = StatusPresenca.Presente
            }).ToList()
        };

        return View(vm);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarRelatorio(RelatorioCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var idUsuarioClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(idUsuarioClaim))
            return Unauthorized();

        int idUsuario = int.Parse(idUsuarioClaim);

        foreach (var item in model.Casais)
        {
            var relatorio = new RelatorioSemanal
            {
                IdMatricula = item.IdMatricula,
                IdLicao = model.IdLicao,
                Presenca = item.Presenca,
                Observacoes = item.Observacoes ?? string.Empty,
                IdUsuario = idUsuario,
                DataRegistro = DateTime.Now,
                DataLicao = model.DataLicao
            };

            await _service.CriarRelatorioAsync(relatorio);
        }

        return RedirectToAction("Index");
    }

}
