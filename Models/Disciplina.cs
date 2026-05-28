namespace Academico.Models;

public class Disciplina
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int CargaHoraria { get; set; }
    public int? ProfessorId { get; set; }
    public Professor? Professor { get; set; }
    public List<Aluno> Alunos { get; set; } = [];
    public string Periodo { get; set; } = string.Empty;
}
