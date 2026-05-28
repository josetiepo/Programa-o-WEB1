using Academico.Interfaces;
using Academico.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academico.Controllers;

[Authorize]
public class AlunoController : Controller
{
    private readonly IAlunoRepository _alunoRepository;

    public AlunoController(IAlunoRepository alunoRepository)
    {
        _alunoRepository = alunoRepository;
    }

    public async Task<IActionResult> Index()
    {
        var alunos = await _alunoRepository.GetAllAlunosAsync();
        return View(alunos);
    }

    public IActionResult CriarAluno()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("CriarAluno")]
    public async Task<IActionResult> CriarAlunoPostAsync(Aluno aluno)
    {
        await ValidarAlunoAsync(aluno);

        if (!ModelState.IsValid)
        {
            return View(aluno);
        }

        if (await _alunoRepository.CriarAlunoAsync(aluno))
        {
            TempData["Tipo"] = "success";
            TempData["Mensagem"] = $"Aluno {aluno.Nome} cadastrado com sucesso!";
        }
        else
        {
            TempData["Tipo"] = "danger";
            TempData["Mensagem"] = $"Aluno {aluno.Nome} nao foi cadastrado.";
        }

        return RedirectToAction(nameof(CriarAluno));
    }

    public async Task<IActionResult> AtualizarAluno(int id)
    {
        if (id <= 0)
        {
            TempData["Tipo"] = "warning";
            TempData["Mensagem"] = "Selecione um aluno na lista para atualizar.";
            return RedirectToAction(nameof(Index));
        }

        var aluno = await _alunoRepository.ObterAlunoPorIdAsync(id);
        if (aluno is null)
        {
            TempData["Tipo"] = "danger";
            TempData["Mensagem"] = "Aluno nao encontrado.";
            return RedirectToAction(nameof(Index));
        }

        return View(aluno);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("AtualizarAluno")]
    public async Task<IActionResult> AtualizarAlunoPostAsync(Aluno aluno)
    {
        await ValidarAlunoAsync(aluno, aluno.Id);

        if (!ModelState.IsValid)
        {
            return View(aluno);
        }

        if (await _alunoRepository.AtualizarAlunoAsync(aluno))
        {
            TempData["Tipo"] = "success";
            TempData["Mensagem"] = $"Aluno {aluno.Nome} atualizado com sucesso!";
        }
        else
        {
            TempData["Tipo"] = "danger";
            TempData["Mensagem"] = $"Aluno {aluno.Nome} nao foi atualizado.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirAluno(int id)
    {
        return await ExcluirAlunoAsync(id);
    }

    [HttpGet]
    [ActionName("ExcluirAluno")]
    public async Task<IActionResult> ExcluirAlunoGet(int id)
    {
        return await ExcluirAlunoAsync(id);
    }

    private async Task<IActionResult> ExcluirAlunoAsync(int id)
    {
        if (id <= 0)
        {
            TempData["Tipo"] = "warning";
            TempData["Mensagem"] = "Selecione um aluno valido para excluir.";
            return RedirectToAction(nameof(Index));
        }

        if (await _alunoRepository.ExcluirAlunoAsync(id))
        {
            TempData["Tipo"] = "success";
            TempData["Mensagem"] = "Aluno excluido com sucesso!";
        }
        else
        {
            TempData["Tipo"] = "danger";
            TempData["Mensagem"] = "Nao foi possivel excluir o aluno.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task ValidarAlunoAsync(Aluno aluno, int? ignorarId = null)
    {
        if (aluno.DataNascimento == default || aluno.DataNascimento >= DateOnly.FromDateTime(DateTime.Today))
        {
            ModelState.AddModelError(nameof(aluno.DataNascimento), "Informe uma data de nascimento valida.");
        }

        if (ModelState.IsValid && await _alunoRepository.CpfEmUsoAsync(aluno.Cpf, ignorarId))
        {
            ModelState.AddModelError(nameof(aluno.Cpf), "Ja existe um aluno cadastrado com este CPF.");
        }
    }
}
