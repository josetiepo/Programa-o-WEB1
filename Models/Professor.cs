using System.ComponentModel.DataAnnotations;

namespace Academico.Models;

public class Professor : Pessoa
{
    [Required(ErrorMessage = "Informe o SIAPE.")]
    [StringLength(30, MinimumLength = 4, ErrorMessage = "O SIAPE deve ter entre 4 e 30 caracteres.")]
    public string Siape { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione a area.")]
    [StringLength(80, ErrorMessage = "A area deve ter no maximo 80 caracteres.")]
    public string Area { get; set; } = string.Empty;
}
