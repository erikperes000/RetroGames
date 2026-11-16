namespace RetroGames.WebApi.ApiModels.Requests;

public class InserirJogoRequestApiModel
{
    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public ushort AnoLancamento { get; set; }

    public decimal Preco { get; set; }

    public uint Estoque { get; set; }

    public uint IdPlataforma { get; set; }

    public ushort IdCondicaoJogo { get; set; }

    public List<uint> IdsGeneros { get; set; } = new();
}
