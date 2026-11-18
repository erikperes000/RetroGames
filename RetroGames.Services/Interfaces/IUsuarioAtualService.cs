namespace RetroGames.Services.Interfaces;

public interface IUsuarioAtualService
{
    bool EstaAutenticado { get; }

    uint? IdUsuario { get; }

    bool EhAdministrador { get; }
}