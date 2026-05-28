using Academico.Models;

namespace Academico.Interfaces;

public interface IDisciplinaRepository
{
    Task<bool> CriarDisciplinaAsync(Disciplina disciplina, int professorId);
    Task<List<Disciplina>> GetAllDisciplinasAsync();
    Task<bool> AtualizarDisciplinaAsync(Disciplina disciplina);
    Task<bool> ExcluirDisciplinaAsync(int id);
}
