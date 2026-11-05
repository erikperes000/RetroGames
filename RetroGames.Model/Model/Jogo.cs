namespace RetroGames.Model.Model
{
    public class Jogo{
        // Coluna: co_seq_jogo
        public uint CoSeqJogo { get; set; }

        // Coluna: no_jogo
        public string NoJogo { get; set; } = string.Empty;

        // Coluna: ds_jogo
        public string? DsJogo { get; set; }

        // Coluna: nu_ano_lancamento
        public ushort NuAnoLancamento { get; set; }

        // Coluna: vl_preco
        public decimal VlPreco { get; set; }

        // Coluna: qt_estoque
        public uint QtEstoque { get; set; }

        // Coluna: st_ativo
        public bool StAtivo { get; set; }

        // Coluna: dt_cadastro
        public DateTime DtCadastro { get; set; }

        // Coluna: co_plataforma
        public uint CoPlataforma { get; set; }

        // Coluna: co_condicao_jogo
        public ushort CoCondicaoJogo { get; set; }
    }
}