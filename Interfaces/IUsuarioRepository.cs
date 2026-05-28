using Academico.Models;

namespace Academico.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task GarantirUsuarioPadraoAsync(UsuarioPadraoOptions usuarioPadrao);
}
