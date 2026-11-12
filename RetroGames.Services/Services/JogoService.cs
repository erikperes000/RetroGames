using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Interfaces;

namespace RetroGames.Services;

public class JogoService : IJogoService{
    private readonly IRepositorySet repositorio;

    public JogoService(IRepositorySet repositorio)
    {
        this.repositorio = repositorio;
    }

    // LISTAR JOGOS
    public async Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        int pagina,
        int tamanhoPagina,
        string? nome = null,
        uint? idPlataforma = null,
        uint? idGenero = null,
        bool incluirInativos = false)
    {
        List<uint>? idsJogos = null;

        // Filtro por gênero utilizando LINQ.
        if (idGenero.HasValue)
        {
            var vinculos = await repositorio.JogoGenero.ListarTodosAsync();

            idsJogos = vinculos
                .Where(v => v.IdGenero == idGenero.Value)
                .Select(v => v.IdJogo)
                .Distinct()
                .ToList();
        }

        var (jogos, total) = await repositorio.Jogo.ListarAsync(
            pagina,
            tamanhoPagina,
            nome,
            idPlataforma,
            idsJogos,
            incluirInativos
        );

        var detalhes = await MontarDetalhesAsync(jogos);

        return (detalhes, total);
    }

    // BUSCAR JOGO POR ID
    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(uint id)
    {
        var jogo = await repositorio.Jogo.BuscarPorIdAsync(id);

        if (jogo == null)
            throw new BusinessRuleException(BusinessErrors.JOGO_NAO_ENCONTRADO);

        var detalhes = await MontarDetalhesAsync(new List<Jogo> { jogo });

        return detalhes.Single();
    }

    // INSERIR JOGO
    public async Task<JogoDetalheServiceModel> InserirAsync(
        Jogo jogo,
        List<uint> idsGeneros)
    {
        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = true;
        jogo.DataCadastro = DateTime.UtcNow;

        await ValidarReferenciasAsync(jogo, idsGeneros);

        var idsGenerosUnicos = idsGeneros.Distinct().ToList();

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        using (var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            transactionOptions,
            TransactionScopeAsyncFlowOption.Enabled))
        {
            jogo.Id = await repositorio.Jogo.InserirAsync(jogo);

            foreach (var idGenero in idsGenerosUnicos)
            {
                await repositorio.JogoGenero.InserirAsync(
                    jogo.Id,
                    idGenero
                );
            }

            transactionScope.Complete();
        }

        return await BuscarPorIdAsync(jogo.Id);
    }

    // ALTERAR JOGO
    public async Task<JogoDetalheServiceModel> AlterarAsync(
        Jogo jogo,
        List<uint> idsGeneros)
    {
        var jogoExistente = await repositorio.Jogo.BuscarPorIdAsync(jogo.Id);

        if (jogoExistente == null)
            throw new BusinessRuleException(BusinessErrors.JOGO_NAO_ENCONTRADO);

        jogo.Nome = jogo.Nome.Trim();

        await ValidarReferenciasAsync(jogo, idsGeneros);

        var idsGenerosUnicos = idsGeneros.Distinct().ToList();

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        using (var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            transactionOptions,
            TransactionScopeAsyncFlowOption.Enabled))
        {
            await repositorio.Jogo.AlterarAsync(jogo);

            await repositorio.JogoGenero.RemoverPorJogoAsync(jogo.Id);

            foreach (var idGenero in idsGenerosUnicos)
            {
                await repositorio.JogoGenero.InserirAsync(
                    jogo.Id,
                    idGenero
                );
            }

            transactionScope.Complete();
        }

        return await BuscarPorIdAsync(jogo.Id);
    }

    // DESATIVAR JOGO
    public async Task DesativarAsync(uint id)
    {
        var jogo = await repositorio.Jogo.BuscarPorIdAsync(id);

        if (jogo == null)
            throw new BusinessRuleException(BusinessErrors.JOGO_NAO_ENCONTRADO);

        await repositorio.Jogo.DesativarAsync(id);
    }

    // VALIDAR REFERÊNCIAS
    private async Task ValidarReferenciasAsync(
        Jogo jogo,
        List<uint> idsGeneros)
    {
        var plataforma = await repositorio.Plataforma.BuscarPorIdAsync(
            jogo.IdPlataforma
        );

        if (plataforma == null)
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NAO_ENCONTRADA
            );

        var condicoes = await repositorio.CondicaoJogo.ListarAsync();

        if (!condicoes.Any(c => c.Id == jogo.IdCondicaoJogo))
            throw new BusinessRuleException(
                BusinessErrors.CONDICAO_NAO_ENCONTRADA
            );

        var generos = await repositorio.Genero.ListarAsync();

        foreach (var idGenero in idsGeneros.Distinct())
        {
            if (!generos.Any(g => g.Id == idGenero))
                throw new BusinessRuleException(
                    BusinessErrors.GENERO_NAO_ENCONTRADO
                );
        }
    }

    // MONTAR DETALHES DOS JOGOS COM LINQ
    private async Task<List<JogoDetalheServiceModel>> MontarDetalhesAsync(
        List<Jogo> jogos)
    {
        if (jogos.Count == 0)
            return new List<JogoDetalheServiceModel>();

        var plataformas = await repositorio.Plataforma.ListarAsync();

        var condicoes = await repositorio.CondicaoJogo.ListarAsync();

        var generos = await repositorio.Genero.ListarAsync();

        var idsJogos = jogos
            .Select(j => j.Id)
            .ToList();

        var vinculos = await repositorio.JogoGenero.ListarPorJogosAsync(
            idsJogos
        );

        var detalhes =
            (from jogo in jogos
             join plataforma in plataformas
                 on jogo.IdPlataforma equals plataforma.Id
             join condicao in condicoes
                 on jogo.IdCondicaoJogo equals condicao.Id
             select new JogoDetalheServiceModel
             {
                 Jogo = jogo,

                 NomePlataforma = plataforma.Nome,

                 DescricaoCondicao = condicao.Descricao,

                 Generos =
                     (from vinculo in vinculos
                      where vinculo.IdJogo == jogo.Id
                      join genero in generos
                          on vinculo.IdGenero equals genero.Id
                      select genero).ToList()
             }).ToList();

        return detalhes;
    }
}