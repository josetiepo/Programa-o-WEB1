using System.ComponentModel.DataAnnotations;

namespace Academico.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Nome { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(160)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string Perfil { get; set; } = "Usuario";

    public bool Ativo { get; set; } = true;
}
