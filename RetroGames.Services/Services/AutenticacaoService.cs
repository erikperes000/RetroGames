using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Interfaces;

namespace RetroGames.Services.Services;

public class AutenticacaoService : IAutenticacaoService
{
    private readonly IRepositorySet _repositorio;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public AutenticacaoService(
        IRepositorySet repositorio,
        IPasswordHasher<Usuario> passwordHasher
    )
    {
        _repositorio = repositorio;
        _passwordHasher = passwordHasher;
    }

    public async Task<Usuario?> AutenticarAsync(
        string email,
        string senha
    )
    {
        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrEmpty(senha))
        {
            return null;
        }

        var usuario = await _repositorio.Usuario
            .BuscarPorEmailAsync(email.Trim());

        if (usuario == null || !usuario.Ativo)
            return null;

        var resultado = _passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.SenhaHash,
            senha
        );

        if (resultado == PasswordVerificationResult.Failed)
            return null;

        return usuario;
    }

    public async Task GarantirAdministradorInicialAsync(
        string? email,
        string? senha
    )
    {
        var existeAdministrador = await _repositorio.Usuario
            .ExisteAdministradorAsync();

        if (existeAdministrador)
            return;

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(senha))
        {
            throw new InvalidOperationException(
                "Configure AdminInicial__Email e AdminInicial__Senha no arquivo .env."
            );
        }

        var administrador = new Usuario
        {
            Nome = "Administrador",
            Email = email.Trim(),
            Ativo = true,
            DataCadastro = DateTime.UtcNow,
            IdPerfil = Perfis.Administrador
        };

        administrador.SenhaHash = _passwordHasher.HashPassword(
            administrador,
            senha
        );

        administrador.Id = await _repositorio.Usuario
            .InserirAsync(administrador);
    }
}