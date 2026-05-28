using Academico.Interfaces;
using Academico.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academico.Controllers;

[Authorize]
public class ProfessorController : Controller
{
    private readonly IProfessorRepository _professorRepository;

    public ProfessorController(IProfessorRepository professorRepository)
    {
        _professorRepository = professorRepository;
    }

    public async Task<IActionResult> Index()
    {
        var professores = await _professorRepository.GetAllProfessoresAsync();
        return View(professores);
    }

    public IActionResult CriarProfessor()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("CriarProfessor")]
    public async Task<IActionResult> CriarProfessorPostAsync(Professor professor)
    {
        await ValidarProfessorAsync(professor);

        if (!ModelState.IsValid)
        {
            return View(professor);
        }

        if (await _professorRepository.CriarProfessorAsync(professor))
        {
            TempData["Tipo"] = "success";
            TempData["Mensagem"] = $"Professor {professor.Nome} cadastrado com sucesso!";
        }
        else
        {
            TempData["Tipo"] = "danger";
            TempData["Mensagem"] = $"Professor {professor.Nome} nao foi cadastrado.";
        }

        return RedirectToAction(nameof(CriarProfessor));
    }

    public async Task<IActionResult> AtualizarProfessor(int id)
    {
        if (id <= 0)
        {
            TempData["Tipo"] = "warning";
            TempData["Mensagem"] = "Selecione um professor na lista para atualizar.";
            return RedirectToAction(nameof(Index));
        }

        var professor = await _professorRepository.ObterProfessorPorIdAsync(id);
        if (professor is null)
        {
            TempData["Tipo"] = "danger";
            TempData["Mensagem"] = "Professor nao encontrado.";
            return RedirectToAction(nameof(Index));
        }

        return View(professor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("AtualizarProfessor")]
    public async Task<IActionResult> AtualizarProfessorPostAsync(Professor professor)
    {
        await ValidarProfessorAsync(professor, professor.Id);

        if (!ModelState.IsValid)
        {
            return View(professor);
        }

        if (await _professorRepository.AtualizarProfessorAsync(professor))
        {
            TempData["Tipo"] = "success";
            TempData["Mensagem"] = $"Professor {professor.Nome} atualizado com sucesso!";
        }
        else
        {
            TempData["Tipo"] = "danger";
            TempData["Mensagem"] = $"Professor {professor.Nome} nao foi atualizado.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirProfessor(int id)
    {
        return await ExcluirProfessorAsync(id);
    }

    [HttpGet]
    [ActionName("ExcluirProfessor")]
    public async Task<IActionResult> ExcluirProfessorGet(int id)
    {
        return await ExcluirProfessorAsync(id);
    }

    private async Task<IActionResult> ExcluirProfessorAsync(int id)
    {
        if (id <= 0)
        {
            TempData["Tipo"] = "warning";
            TempData["Mensagem"] = "Selecione um professor valido para excluir.";
            return RedirectToAction(nameof(Index));
        }

        if (await _professorRepository.ExcluirProfessorAsync(id))
        {
            TempData["Tipo"] = "success";
            TempData["Mensagem"] = "Professor excluido com sucesso!";
        }
        else
        {
            TempData["Tipo"] = "danger";
            TempData["Mensagem"] = "Nao foi possivel excluir o professor.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task ValidarProfessorAsync(Professor professor, int? ignorarId = null)
    {
        if (professor.DataNascimento == default || professor.DataNascimento >= DateOnly.FromDateTime(DateTime.Today))
        {
            ModelState.AddModelError(nameof(professor.DataNascimento), "Informe uma data de nascimento valida.");
        }

        if (ModelState.IsValid && await _professorRepository.CpfEmUsoAsync(professor.Cpf, ignorarId))
        {
            ModelState.AddModelError(nameof(professor.Cpf), "Ja existe um professor cadastrado com este CPF.");
        }

        if (ModelState.IsValid && await _professorRepository.SiapeEmUsoAsync(professor.Siape, ignorarId))
        {
            ModelState.AddModelError(nameof(professor.Siape), "Ja existe um professor cadastrado com este SIAPE.");
        }
    }
}
