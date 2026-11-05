namespace RetroGames.Model.Model
{
    public class Plataforma{
        // Coluna: co_seq_plataforma
        public uint CoSeqPlataforma { get; set; }

        // Coluna: no_plataforma
        public string NoPlataforma { get; set; } = string.Empty;

        // Coluna: no_fabricante
        public string NoFabricante { get; set; } = string.Empty;

        // Coluna: nu_ano_lancamento
        public ushort NuAnoLancamento { get; set; }
    }
}