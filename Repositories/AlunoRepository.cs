using Academico.Models;
using Academico.Data;
using Academico.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Academico.Repositories;

public class AlunoRepository : IAlunoRepository
{
    private readonly AppDbContext _context;

    public AlunoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CriarAlunoAsync(Aluno aluno)
    {
        var totalAlunos = await _context.Alunos.CountAsync();
        aluno.Nome = aluno.Nome.Trim();
        aluno.Cpf = NormalizarCpf(aluno.Cpf);
        aluno.Curso = aluno.Curso.Trim();
        aluno.Matricula = $"{DateTime.Now.Year}{totalAlunos + 1:D4}";
        await _context.Alunos.AddAsync(aluno);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Aluno>> GetAllAlunosAsync()
    {
        return await _context.Alunos.OrderBy(x => x.Nome).ToListAsync();
    }

    public async Task<Aluno?> ObterAlunoPorIdAsync(int id)
    {
        return await _context.Alunos.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> AtualizarAlunoAsync(Aluno aluno)
    {
        var alunoBanco = await _context.Alunos.FirstOrDefaultAsync(x => x.Id == aluno.Id);
        if (alunoBanco is null)
        {
            return false;
        }

        alunoBanco.Nome = aluno.Nome.Trim();
        alunoBanco.Cpf = NormalizarCpf(aluno.Cpf);
        alunoBanco.Curso = aluno.Curso.Trim();
        alunoBanco.DataNascimento = aluno.DataNascimento;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExcluirAlunoAsync(int id)
    {
        var aluno = await _context.Alunos
            .Include(x => x.Disciplinas)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (aluno is null)
        {
            return false;
        }

        aluno.Disciplinas.Clear();
        _context.Alunos.Remove(aluno);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CpfEmUsoAsync(string cpf, int? ignorarId = null)
    {
        var cpfNormalizado = NormalizarCpf(cpf);
        return await _context.Alunos.AnyAsync(x => x.Cpf == cpfNormalizado && (!ignorarId.HasValue || x.Id != ignorarId.Value));
    }

    private static string NormalizarCpf(string cpf)
    {
        var numeros = new string(cpf.Where(char.IsDigit).ToArray());
        return numeros.Length == 11
            ? $"{numeros[..3]}.{numeros.Substring(3, 3)}.{numeros.Substring(6, 3)}-{numeros[9..]}"
            : cpf.Trim();
    }
}
