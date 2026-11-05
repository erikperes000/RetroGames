namespace RetroGames.Model.Model{
    public class Genero{
        // Coluna: co_seq_genero (chave primária)
        public uint Id { get; set; }

        // Coluna: no_genero
        public string Nome { get; set; } = string.Empty;
    }
}