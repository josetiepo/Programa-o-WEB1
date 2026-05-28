using System.ComponentModel.DataAnnotations;

namespace Academico.Models;

public class Aluno : Pessoa
{
    public string Matricula { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione o curso.")]
    [StringLength(80, ErrorMessage = "O curso deve ter no maximo 80 caracteres.")]
    public string Curso { get; set; } = string.Empty;

    public List<Disciplina> Disciplinas { get; set; } = [];
}
