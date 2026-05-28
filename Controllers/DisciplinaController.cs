using Academico.Interfaces;
using Academico.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Academico.Controllers;

[Authorize]
public class DisciplinaController : Controller
{
    private readonly IDisciplinaRepository _disciplinaRepository;
    private readonly IProfessorRepository _professorRepository;

    public DisciplinaController(IDisciplinaRepository disciplinaRepository, IProfessorRepository professorRepository)
    {
        _disciplinaRepository = disciplinaRepository;
        _professorRepository = professorRepository;
    }

    public async Task<IActionResult> Index()
    {
        var disciplinas = await _disciplinaRepository.GetAllDisciplinasAsync();
        return View(disciplinas);
    }

    public async Task<IActionResult> CriarDisciplina()
    {
        await CarregarProfessoresAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("CriarDisciplina")]
    public async Task<IActionResult> CriarDisciplinaPostAsync(DisciplinaViewModel disciplinaViewModel)
    {
        if (!ModelState.IsValid)
        {
            await CarregarProfessoresAsync();
            return View(disciplinaViewModel);
        }

        var disciplina = new Disciplina
        {
            Nome = disciplinaViewModel.Nome,
            CargaHoraria = disciplinaViewModel.CargaHoraria,
            Periodo = disciplinaViewModel.Periodo
        };

        await _disciplinaRepository.CriarDisciplinaAsync(disciplina, disciplinaViewModel.ProfessorSelecionadoId);

        TempData["Tipo"] = "success";
        TempData["Mensagem"] = $"Disciplina {disciplina.Nome} cadastrada com sucesso!";
        return RedirectToAction(nameof(CriarDisciplina));
    }

    public IActionResult AtualizarDisciplina()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("AtualizarDisciplina")]
    public async Task<IActionResult> AtualizarDisciplinaPostAsync(Disciplina disciplina)
    {
        if (await _disciplinaRepository.AtualizarDisciplinaAsync(disciplina))
        {
            TempData["Tipo"] = "success";
            TempData["Mensagem"] = $"Disciplina {disciplina.Nome} atualizada com sucesso!";
        }
        else
        {
            TempData["Tipo"] = "danger";
            TempData["Mensagem"] = $"Disciplina {disciplina.Nome} nao foi atualizada.";
        }

        return RedirectToAction(nameof(AtualizarDisciplina));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirDisciplina(int id)
    {
        return await ExcluirDisciplinaAsync(id);
    }

    [HttpGet]
    [ActionName("ExcluirDisciplina")]
    public async Task<IActionResult> ExcluirDisciplinaGet(int id)
    {
        return await ExcluirDisciplinaAsync(id);
    }

    private async Task<IActionResult> ExcluirDisciplinaAsync(int id)
    {
        if (id <= 0)
        {
            TempData["Tipo"] = "warning";
            TempData["Mensagem"] = "Selecione uma disciplina valida para excluir.";
            return RedirectToAction(nameof(Index));
        }

        if (await _disciplinaRepository.ExcluirDisciplinaAsync(id))
        {
            TempData["Tipo"] = "success";
            TempData["Mensagem"] = "Disciplina excluida com sucesso!";
        }
        else
        {
            TempData["Tipo"] = "danger";
            TempData["Mensagem"] = "Nao foi possivel excluir a disciplina.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task CarregarProfessoresAsync()
    {
        ViewBag.Professor = new SelectList(
            await _professorRepository.GetAllProfessoresAsync(),
            "Id",
            "Nome");
    }
}
