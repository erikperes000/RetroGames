using System.Security.Claims;
using RetroGames.Model.Constantes;
using RetroGames.Services.Interfaces;

namespace RetroGames.WebApi.Services;

public class UsuarioAtualService : IUsuarioAtualService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioAtualService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Usuario =>
        _httpContextAccessor.HttpContext?.User;

    public bool EstaAutenticado =>
        Usuario?.Identity?.IsAuthenticated == true;

    public uint? IdUsuario
    {
        get
        {
            var id = Usuario?.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (uint.TryParse(id, out var idUsuario))
                return idUsuario;

            return null;
        }
    }

    public bool EhAdministrador =>
        Usuario?.IsInRole(
            nameof(Perfis.Administrador)) == true;
}