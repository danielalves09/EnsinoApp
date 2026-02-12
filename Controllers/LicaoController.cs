using EnsinoApp.Models.Entities;
using EnsinoApp.Services.Cursos;
using EnsinoApp.ViewModels.Licao;
using Microsoft.AspNetCore.Mvc;
using EnsinoApp.Services.Licao;
using Microsoft.AspNetCore.Authorization;

namespace EnsinoApp.Controllers
{
    [Authorize]
    public class LicaoController : Controller
    {
        private readonly ILicaoService _service;
        private readonly ICursoService _cursoService;

        public LicaoController(ILicaoService service, ICursoService cursoService)
        {
            _service = service;
            _cursoService = cursoService;
        }

        // Lista todas as lições de um curso
        public async Task<IActionResult> Index(int cursoId)
        {
            var curso = _cursoService.FindById(cursoId);
            if (curso == null) return NotFound();

            var licoes = await _service.FindByCursoAsync(cursoId);
            var vm = licoes.Select(l => new LicaoViewModel
            {
                Id = l.Id,
                IdCurso = l.IdCurso,
                Numero = l.NumeroSemana,
                Titulo = l.Titulo,
                Descricao = l.Descricao,
            }).ToList();

            ViewBag.CursoNome = curso.Nome;
            ViewBag.CursoId = cursoId;

            return View(vm);
        }

        // Formulário de criação
        public IActionResult Adicionar(int cursoId)
        {
            var vm = new LicaoViewModel { IdCurso = cursoId, DataAula = DateTime.Today };
            ViewBag.CursoId = cursoId;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adicionar(LicaoViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var licao = new Licao
            {
                IdCurso = model.IdCurso,
                NumeroSemana = model.Numero,
                Titulo = model.Titulo,
                Descricao = model.Descricao,
            };

            await _service.CreateAsync(licao);
            return RedirectToAction(nameof(Index), new { cursoId = model.IdCurso });
        }

        // Formulário de edição
        public async Task<IActionResult> Editar(int id)
        {
            var licao = await _service.FindByIdAsync(id);
            if (licao == null) return NotFound();

            var vm = new LicaoViewModel
            {
                Id = licao.Id,
                IdCurso = licao.IdCurso,
                Numero = licao.NumeroSemana,
                Titulo = licao.Titulo,
                Descricao = licao.Descricao,
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(LicaoViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var licao = await _service.FindByIdAsync(model.Id);
            if (licao == null) return NotFound();

            licao.NumeroSemana = model.Numero;
            licao.Titulo = model.Titulo;
            licao.Descricao = model.Descricao;

            await _service.UpdateAsync(licao);
            return RedirectToAction(nameof(Index), new { cursoId = model.IdCurso });
        }

        // Exclusão
        public async Task<IActionResult> Deletar(int id)
        {
            var licao = await _service.FindByIdAsync(id);
            if (licao == null) return NotFound();

            return View(new LicaoViewModel
            {
                Id = licao.Id,
                IdCurso = licao.IdCurso,
                Numero = licao.NumeroSemana,
                Titulo = licao.Titulo,
                Descricao = licao.Descricao,
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index), new { cursoId = (await _service.FindByIdAsync(id))?.IdCurso });
        }
    }
}
