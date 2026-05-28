using Academico.Models;

namespace Academico.Interfaces;

public interface IProfessorRepository
{
    Task<bool> CriarProfessorAsync(Professor professor);
    Task<List<Professor>> GetAllProfessoresAsync();
    Task<Professor?> ObterProfessorPorIdAsync(int id);
    Task<bool> AtualizarProfessorAsync(Professor professor);
    Task<bool> ExcluirProfessorAsync(int id);
    Task<bool> CpfEmUsoAsync(string cpf, int? ignorarId = null);
    Task<bool> SiapeEmUsoAsync(string siape, int? ignorarId = null);
}
