using EnsinoApp.Models.Entities;
using EnsinoApp.Models.Enums;
using EnsinoApp.Services.Casal;
using EnsinoApp.Services.Inscricao;
using EnsinoApp.Services.Matricula;
using EnsinoApp.Services.Turmas;
using EnsinoApp.ViewModels.Matricula;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnsinoApp.Controllers;

public class MatriculaController : Controller
{
    private readonly ICasalService _casalService;
    private readonly IInscricaoOnlineService _inscricaoService;
    private readonly IMatriculaService _matriculaService;
    private readonly ITurmaService _turmaService;

    public MatriculaController(
            ICasalService casalService,
            IInscricaoOnlineService inscricaoService,
            IMatriculaService matriculaService,
            ITurmaService turmaService)
    {
        _casalService = casalService;
        _inscricaoService = inscricaoService;
        _matriculaService = matriculaService;
        _turmaService = turmaService;
    }

    public IActionResult Index()
    {
        var dashboard = new MatriculaDashboardViewModel
        {
            TotalCasais = _casalService.ContarTotal(),
            TotalInscricoes = _inscricaoService.ContarTotal(),
            InscricoesPendentes = _inscricaoService.ContarPendentes(),
            MatriculasAtivas = _matriculaService.ContarAtivas(),
            TurmasAtivas = _turmaService.ContarAtivas(),

            InscricoesPendentesLista = _inscricaoService.ObterPendentesResumo(),
            Casais = _casalService.ObterResumoCasais(),
            Turmas = _turmaService.ObterResumoTurmasAtivas()
        };

        return View(dashboard);
    }

    public async Task<IActionResult> Cadastrar(int id)
    {
        var inscricao = await _inscricaoService.FindByIdAsync(id);
        if (inscricao == null)
            return NotFound();

        var model = new MatriculaFormViewModel
        {
            IdInscricao = inscricao.Id,
            NomeCasal = $"{inscricao.NomeMarido} e {inscricao.NomeEsposa}",
            NomeGC = inscricao.NomeGC,
            NomeCampus = inscricao.Campus.Nome,
            SelectTurmas = new SelectList(await _turmaService.FindAllAtivasAsync(inscricao.IdCurso), "Id", "Descricao")
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cadastrar(MatriculaFormViewModel model)
    {
        Console.WriteLine("Entrou na função");
        if (!ModelState.IsValid)
        {
            model.SelectTurmas = new SelectList(await _turmaService.FindAllAtivasAsync(model.IdCurso), "Id", "Descricao");
            return View(model);
        }

        var inscricao = await _inscricaoService.FindByIdAsync(model.IdInscricao);
        if (inscricao == null)
            return NotFound();

        var casal = new Casal
        {
            NomeConjuge1 = inscricao.NomeMarido,
            NomeConjuge2 = inscricao.NomeEsposa,
            TelefoneConjuge1 = inscricao.TelefoneMarido,
            TelefoneConjuge2 = inscricao.TelefoneEsposa,
            EmailConjuge1 = inscricao.EmailMarido,
            EmailConjuge2 = inscricao.EmailEsposa,
            Rua = inscricao.Rua,
            Numero = inscricao.Numero,
            Complemento = inscricao.Complemento,
            Bairro = inscricao.Bairro,
            Cidade = inscricao.Cidade,
            Estado = inscricao.Estado,
            Cep = inscricao.Cep,
            Status = StatusCasal.Ativo,
            IdCampus = inscricao.IdCampus
        };


        casal = await _casalService.CreateAsync(casal);


        var matricula = new Matricula
        {
            IdCasal = casal.Id,
            IdTurma = model.IdTurma,
            NomeGC = inscricao.NomeGC,
            Status = StatusMatricula.Ativa,
            DataMatricula = DateTime.Now
        };

        await _matriculaService.CreateAsync(matricula);


        inscricao.Processada = true;
        await _inscricaoService.UpdateAsync(inscricao);



        return RedirectToAction("Index");
    }








}