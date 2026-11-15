namespace RetroGames.WebApi.ApiModels.Responses;

public class ListarPlataformaResponseApiModel
{
    public uint Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Fabricante { get; set; } = string.Empty;

    public ushort AnoLancamento { get; set; }
}