using Academico.Models;
using Academico.Data;
using Academico.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Academico.Repositories;

public class ProfessorRepository : IProfessorRepository
{
    private readonly AppDbContext _context;

    public ProfessorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CriarProfessorAsync(Professor professor)
    {
        professor.Nome = professor.Nome.Trim();
        professor.Cpf = NormalizarCpf(professor.Cpf);
        professor.Siape = professor.Siape.Trim();
        professor.Area = professor.Area.Trim();
        await _context.Professores.AddAsync(professor);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Professor>> GetAllProfessoresAsync()
    {
        return await _context.Professores.OrderBy(x => x.Nome).ToListAsync();
    }

    public async Task<Professor?> ObterProfessorPorIdAsync(int id)
    {
        return await _context.Professores.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> AtualizarProfessorAsync(Professor professor)
    {
        var professorBanco = await _context.Professores.FirstOrDefaultAsync(x => x.Id == professor.Id);
        if (professorBanco is null)
        {
            return false;
        }

        professorBanco.Nome = professor.Nome.Trim();
        professorBanco.Cpf = NormalizarCpf(professor.Cpf);
        professorBanco.Siape = professor.Siape.Trim();
        professorBanco.DataNascimento = professor.DataNascimento;
        professorBanco.Area = professor.Area.Trim();
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExcluirProfessorAsync(int id)
    {
        var professor = await _context.Professores.FirstOrDefaultAsync(x => x.Id == id);

        if (professor is null)
        {
            return false;
        }

        var disciplinas = await _context.Disciplinas
            .Where(x => x.ProfessorId == id)
            .ToListAsync();

        foreach (var disciplina in disciplinas)
        {
            disciplina.ProfessorId = null;
        }

        _context.Professores.Remove(professor);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CpfEmUsoAsync(string cpf, int? ignorarId = null)
    {
        var cpfNormalizado = NormalizarCpf(cpf);
        return await _context.Professores.AnyAsync(x => x.Cpf == cpfNormalizado && (!ignorarId.HasValue || x.Id != ignorarId.Value));
    }

    public async Task<bool> SiapeEmUsoAsync(string siape, int? ignorarId = null)
    {
        var siapeNormalizado = siape.Trim();
        return await _context.Professores.AnyAsync(x => x.Siape == siapeNormalizado && (!ignorarId.HasValue || x.Id != ignorarId.Value));
    }

    private static string NormalizarCpf(string cpf)
    {
        var numeros = new string(cpf.Where(char.IsDigit).ToArray());
        return numeros.Length == 11
            ? $"{numeros[..3]}.{numeros.Substring(3, 3)}.{numeros.Substring(6, 3)}-{numeros[9..]}"
            : cpf.Trim();
    }
}
