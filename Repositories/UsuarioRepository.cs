using Academico.Data;
using Academico.Interfaces;
using Academico.Models;
using Academico.Services;
using Microsoft.EntityFrameworkCore;

namespace Academico.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public UsuarioRepository(AppDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public Task<Usuario?> ObterPorEmailAsync(string email)
    {
        return _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email && x.Ativo);
    }

    public async Task GarantirUsuarioPadraoAsync(UsuarioPadraoOptions usuarioPadrao)
    {
        if (string.IsNullOrWhiteSpace(usuarioPadrao.Email) || string.IsNullOrWhiteSpace(usuarioPadrao.Senha))
        {
            return;
        }

        var emails = usuarioPadrao.EmailsLegados
            .Append(usuarioPadrao.Email)
            .Where(email => !string.IsNullOrWhiteSpace(email))
            .Select(email => email.Trim())
            .Distinct()
            .ToList();

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(x => emails.Contains(x.Email));

        if (usuario is null)
        {
            usuario = new Usuario();
            await _context.Usuarios.AddAsync(usuario);
        }

        usuario.Nome = usuarioPadrao.Nome.Trim();
        usuario.Email = usuarioPadrao.Email.Trim();
        usuario.PasswordHash = _passwordHasher.Hash(usuarioPadrao.Senha);
        usuario.Perfil = usuarioPadrao.Perfil.Trim();
        usuario.Ativo = true;

        await _context.SaveChangesAsync();
    }
}
