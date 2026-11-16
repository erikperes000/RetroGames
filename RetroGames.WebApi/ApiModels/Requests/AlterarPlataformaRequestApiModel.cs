namespace RetroGames.WebApi.ApiModels.Requests;

public class AlterarPlataformaRequestApiModel
{
    public string Nome { get; set; } = string.Empty;

    public string Fabricante { get; set; } = string.Empty;

    public ushort AnoLancamento { get; set; }
}