namespace RetroGames.WebApi.ApiModels.Requests;

public class InserirPlataformaRequestApiModel
{
    public string Nome { get; set; } = string.Empty;

    public string Fabricante { get; set; } = string.Empty;

    public ushort AnoLancamento { get; set; }
}