using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> BuscarPorEmailAsync(string email);

    Task<Usuario?> BuscarPorIdAsync(uint id);

    Task<bool> ExisteAdministradorAsync();

    Task<uint> InserirAsync(Usuario usuario);
}
