namespace RetroGames.WebApi.ApiModels.Responses;

public class ListarPorIdJogoResponseApiModel
{
    public uint Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public ushort AnoLancamento { get; set; }
    public decimal Preco { get; set; }
    public uint Estoque { get; set; }
    public bool Ativo { get; set; }

    public uint IdPlataforma { get; set; }
    public string NomePlataforma { get; set; } = string.Empty;

    public ushort IdCondicaoJogo { get; set; }
    public string DescricaoCondicao { get; set; } = string.Empty;

    public List<ListarGeneroResponseApiModel> Generos { get; set; } = new();
}