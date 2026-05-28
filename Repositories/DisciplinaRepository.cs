using Academico.Models;
using Academico.Data;
using Academico.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Academico.Repositories;

public class DisciplinaRepository : IDisciplinaRepository
{
    private readonly AppDbContext _context;

    public DisciplinaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CriarDisciplinaAsync(Disciplina disciplina, int professorId)
    {
        disciplina.ProfessorId = professorId;
        await _context.Disciplinas.AddAsync(disciplina);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Disciplina>> GetAllDisciplinasAsync()
    {
        return await _context.Disciplinas
            .Include(x => x.Professor)
            .OrderBy(x => x.Nome)
            .ToListAsync();
    }

    public async Task<bool> AtualizarDisciplinaAsync(Disciplina disciplina)
    {
        var disciplinaBanco = await _context.Disciplinas.FirstOrDefaultAsync(x => x.Id == disciplina.Id);
        if (disciplinaBanco is null)
        {
            return false;
        }

        disciplinaBanco.Nome = disciplina.Nome;
        disciplinaBanco.CargaHoraria = disciplina.CargaHoraria;
        disciplinaBanco.ProfessorId = disciplina.ProfessorId;
        disciplinaBanco.Periodo = disciplina.Periodo;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExcluirDisciplinaAsync(int id)
    {
        var disciplina = await _context.Disciplinas
            .Include(x => x.Alunos)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (disciplina is null)
        {
            return false;
        }

        disciplina.Alunos.Clear();
        _context.Disciplinas.Remove(disciplina);
        await _context.SaveChangesAsync();
        return true;
    }
}
