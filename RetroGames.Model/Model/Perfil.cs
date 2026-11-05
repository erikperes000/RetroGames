namespace RetroGames.Model.Model{
    public class Perfil{
        // Coluna: co_perfil (chave primária)
        public ushort Id { get; set; }

        // Coluna: ds_perfil
        public string Descricao { get; set; } = string.Empty;
    }
}