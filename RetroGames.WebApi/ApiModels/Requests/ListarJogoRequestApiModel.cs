namespace RetroGames.WebApi.ApiModels.Requests;

public class ListarJogoRequestApiModel
{
    public string? Busca { get; set; }

    public uint? IdPlataforma { get; set; }

    public uint? IdGenero { get; set; }

    public int Pagina { get; set; } = 1;

    public int TamanhoPagina { get; set; } = 12;

    public bool IncluirInativos { get; set; } = false;
}