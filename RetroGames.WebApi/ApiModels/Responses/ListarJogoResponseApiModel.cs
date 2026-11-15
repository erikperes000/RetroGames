namespace RetroGames.WebApi.ApiModels.Responses;

public class ListarJogoResponseApiModel
{
    public uint Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public ushort AnoLancamento { get; set; }
    public decimal Preco { get; set; }
    public uint Estoque { get; set; }
    public bool Ativo { get; set; }
    public string NomePlataforma { get; set; } = string.Empty;
    public string DescricaoCondicao { get; set; } = string.Empty;
    public List<string> Generos { get; set; } = new();
}