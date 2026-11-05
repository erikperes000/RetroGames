namespace RetroGames.Model.Model{
    public class Jogo{
        // Coluna: co_seq_jogo (chave primária)
        public uint Id { get; set; }

        // Coluna: no_jogo
        public string Nome { get; set; } = string.Empty;

        // Coluna: ds_jogo (opcional)
        public string? Descricao { get; set; }

        // Coluna: nu_ano_lancamento
        public ushort AnoLancamento { get; set; }

        // Coluna: vl_preco
        public decimal Preco { get; set; }

        // Coluna: qt_estoque
        public uint Estoque { get; set; }

        // Coluna: st_ativo
        public bool Ativo { get; set; }

        // Coluna: dt_cadastro
        public DateTime DataCadastro { get; set; }

        // Coluna: co_plataforma (chave estrangeira)
        public uint IdPlataforma { get; set; }

        // Coluna: co_condicao_jogo (chave estrangeira)
        public ushort IdCondicaoJogo { get; set; }
    }
}