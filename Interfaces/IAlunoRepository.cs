using Academico.Models;

namespace Academico.Interfaces;

public interface IAlunoRepository
{
    Task<bool> CriarAlunoAsync(Aluno aluno);
    Task<List<Aluno>> GetAllAlunosAsync();
    Task<Aluno?> ObterAlunoPorIdAsync(int id);
    Task<bool> AtualizarAlunoAsync(Aluno aluno);
    Task<bool> ExcluirAlunoAsync(int id);
    Task<bool> CpfEmUsoAsync(string cpf, int? ignorarId = null);
}
