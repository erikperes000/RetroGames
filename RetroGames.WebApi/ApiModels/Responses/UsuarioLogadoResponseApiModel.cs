namespace RetroGames.WebApi.ApiModels.Responses;

public class UsuarioLogadoResponseApiModel{
    public uint Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
}