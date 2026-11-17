using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;

namespace RetroGames.WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static ClaimsPrincipal ParaClaimsPrincipal(
        this Usuario usuario)
    {
        var perfil = usuario.IdPerfil == Perfis.Administrador
            ? nameof(Perfis.Administrador)
            : nameof(Perfis.Cliente);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, perfil)
        };

        var identidade = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        return new ClaimsPrincipal(identidade);
    }

    public static uint ObterIdUsuario(this ClaimsPrincipal usuario)
    {
        var id = usuario.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!uint.TryParse(id, out var idUsuario))
            throw new InvalidOperationException(
                "O ID do usuário não foi encontrado nas claims.");

        return idUsuario;
    }
}