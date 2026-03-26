using System.Diagnostics;
using EnsinoApp.Models;
using EnsinoApp.Models.Entities;
using EnsinoApp.Services.Campus;
using EnsinoApp.Services.Casal;
using EnsinoApp.Services.Cursos;
using EnsinoApp.Services.Inscricao;
using EnsinoApp.Services.Matricula;
using EnsinoApp.Services.Supervisao;
using EnsinoApp.Services.Turmas;
using EnsinoApp.Services.Usuarios;
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
        private readonly ICampusService _campusService;
        private readonly ISupervisaoService _supervisaoService;
        private readonly IUsuariosService _usuariosService;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<Usuario> userManager,
            ICasalService casalService,
            IInscricaoOnlineService inscricaoService,
            IMatriculaService matriculaService,
            ITurmaService turmaService,
            ICampusService campusService,
            ISupervisaoService supervisaoService,
            IUsuariosService usuariosService)
        {
            _logger = logger;
            _userManager = userManager;
            _casalService = casalService;
            _inscricaoService = inscricaoService;
            _matriculaService = matriculaService;
            _turmaService = turmaService;
            _campusService = campusService;
            _supervisaoService = supervisaoService;
            _usuariosService = usuariosService;
        }

        public async Task<IActionResult> Index()
        {
            // Dados do usuário logado 
            var user = await _userManager.GetUserAsync(User);

            ViewBag.NomeUsuario = user?.NomeMarido ?? "Visitante";
            ViewBag.FotoPerfil = user?.FotoPerfil;



            var qtdCampus = await _campusService.ContarTotal();
            var qtdSupervisoes = await _supervisaoService.ContarTotal();
            var qtdLideres = await _usuariosService.ContarLideresAsync();


            var matriculasAtivas = await _matriculaService.ContarAtivas();

            ViewBag.QtdCampus = qtdCampus;
            ViewBag.QtdSupervisoes = qtdSupervisoes;
            ViewBag.QtdLideres = qtdLideres;
            ViewBag.QtdCasaisMatriculados = matriculasAtivas;

            // Dados dos gráficos 
            var inscricoesPorMes = await _inscricaoService.GetUltimosMesesAsync(6);
            var matriculasPorCurso = await _matriculaService.GetMatriculasPorCursoAsync();
            var casaisPorCampus = await _casalService.GetCasaisPorCampusAsync();

            // Montar ViewModel 
            var vm = new DashboardViewModel
            {
                TotalCasais = _casalService.ContarTotal(),
                TotalInscricoes = _inscricaoService.ContarTotal(),
                MatriculasAtivas = matriculasAtivas,
                TurmasAtivas = _turmaService.ContarAtivas(),

                InscricoesPorMes = inscricoesPorMes
                    .Select(i => new InscricaoPorMes { MesAno = i.MesAno, Total = i.Total })
                    .ToList(),

                MatriculasPorCurso = matriculasPorCurso
                    .Select(m => new MatriculasPorCurso { Curso = m.Curso, Total = m.Total })
                    .ToList(),

                CasaisPorCampus = casaisPorCampus
                    .Select(c => new CasaisPorCampus { Campus = c.Campus, Total = c.Total })
                    .ToList()
            };

            return View(vm);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
