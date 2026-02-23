using System.Diagnostics;
using EnsinoApp.Models;
using EnsinoApp.Models.Entities;
using EnsinoApp.Services.Campus;
using EnsinoApp.Services.Casal;
using EnsinoApp.Services.Cursos;
using EnsinoApp.Services.Inscricao;
using EnsinoApp.Services.Matricula;
using EnsinoApp.Services.Turmas;
using EnsinoApp.ViewModels.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EnsinoApp.Controllers
{
    [Authorize(Roles = "Admin,Pastor,Coordenador")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<Usuario> _userManager;

        private readonly ICasalService _casalService;
        private readonly IInscricaoOnlineService _inscricaoService;
        private readonly IMatriculaService _matriculaService;
        private readonly ITurmaService _turmaService;
        private readonly ICursoService _cursoService;
        private readonly ICampusService _campusService;

        public HomeController(ILogger<HomeController> logger, UserManager<Usuario> userManager, ICasalService casalService, IInscricaoOnlineService inscricaoService, IMatriculaService matriculaService, ITurmaService turmaService, ICursoService cursoService, ICampusService campusService)
        {
            _logger = logger;
            _userManager = userManager;
            _casalService = casalService;
            _inscricaoService = inscricaoService;
            _matriculaService = matriculaService;
            _turmaService = turmaService;
            _cursoService = cursoService;
            _campusService = campusService;
        }

        public async Task<IActionResult> Index()
        {
            string nomeUsuario = "Visitante";

            if (User.Identity!.IsAuthenticated)
            {
                // Busca o usuário logado pelo Id
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    //Opção para exibir o nome do casal futuramente
                    //nomeUsuario = user.NomeMarido + (string.IsNullOrEmpty(user.NomeEsposa) ? "" : " & " + user.NomeEsposa);
                    nomeUsuario = user.NomeMarido;
                    ViewBag.FotoPerfil = user.FotoPerfil;
                }
            }

            ViewBag.NomeUsuario = nomeUsuario;


            var vm = new DashboardViewModel();

            // KPIs dos cards
            vm.TotalCasais = _casalService.ContarTotal();
            vm.TotalInscricoes = _inscricaoService.ContarTotal();
            vm.MatriculasAtivas = _matriculaService.ContarAtivas();
            vm.TurmasAtivas = _turmaService.ContarAtivas();

            // Gráfico 1: Inscrições por mês (últimos 6 meses)
            var inscricoes = await _inscricaoService.GetUltimosMesesAsync(6);
            vm.InscricoesPorMes = inscricoes.Select(i => new InscricaoPorMes
            {
                MesAno = i.MesAno,
                Total = i.Total
            }).ToList();

            // Gráfico 2: Matrículas por curso
            var matriculasPorCurso = await _matriculaService.GetMatriculasPorCursoAsync();
            vm.MatriculasPorCurso = matriculasPorCurso.Select(m => new MatriculasPorCurso
            {
                Curso = m.Curso,
                Total = m.Total
            }).ToList();

            // Gráfico 3: Casais por campus
            var casaisPorCampus = await _casalService.GetCasaisPorCampusAsync();
            vm.CasaisPorCampus = casaisPorCampus.Select(c => new CasaisPorCampus
            {
                Campus = c.Campus,
                Total = c.Total
            }).ToList();

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
