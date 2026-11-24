using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;
using RetroGames.Services.Interfaces;
using RetroGames.Services.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class JogoServiceTests
{
    private readonly Mock<IRepositorySet> _repositorioMock = new();

    private readonly Mock<IJogoRepository> _jogoRepositoryMock = new();
    private readonly Mock<IPlataformaRepository> _plataformaRepositoryMock = new();
    private readonly Mock<ICondicaoJogoRepository> _condicaoRepositoryMock = new();
    private readonly Mock<IGeneroRepository> _generoRepositoryMock = new();
    private readonly Mock<IJogoGeneroRepository> _jogoGeneroRepositoryMock = new();

    private readonly Mock<IUsuarioAtualService> _usuarioAtualMock = new();

    public JogoServiceTests()
    {
        _repositorioMock.SetupGet(r => r.Jogo)
            .Returns(_jogoRepositoryMock.Object);

        _repositorioMock.SetupGet(r => r.Plataforma)
            .Returns(_plataformaRepositoryMock.Object);

        _repositorioMock.SetupGet(r => r.CondicaoJogo)
            .Returns(_condicaoRepositoryMock.Object);

        _repositorioMock.SetupGet(r => r.Genero)
            .Returns(_generoRepositoryMock.Object);

        _repositorioMock.SetupGet(r => r.JogoGenero)
            .Returns(_jogoGeneroRepositoryMock.Object);

        _usuarioAtualMock.SetupGet(u => u.EhAdministrador)
            .Returns(false);
    }

    private JogoService CriarJogoService()
    {
        return new JogoService(
            _repositorioMock.Object,
            _usuarioAtualMock.Object
        );
    }

    private static Jogo CriarJogo()
    {
        return new Jogo
        {
            Nome = "Super Mario",
            Descricao = "Jogo de plataforma",
            AnoLancamento = 1985,
            Preco = 99.90m,
            Estoque = 10,
            IdPlataforma = 1,
            IdCondicaoJogo = 1
        };
    }

    [Fact]
    public async Task Inserir_PlataformaInexistente_LancaPlataformaNaoEncontrada()
    {
        // Arrange
        var jogo = CriarJogo();
        var idsGeneros = new List<uint> { 1 };

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(jogo.IdPlataforma))
            .ReturnsAsync((Plataforma?)null);

        var service = CriarJogoService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.InserirAsync(jogo, idsGeneros)
        );

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA, excecao.Erro);

        _jogoRepositoryMock.Verify(
            r => r.InserirAsync(It.IsAny<Jogo>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Inserir_CondicaoInexistente_LancaCondicaoNaoEncontrada()
    {
        // Arrange
        var jogo = CriarJogo();
        var idsGeneros = new List<uint> { 1 };

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(jogo.IdPlataforma))
            .ReturnsAsync(new Plataforma { Id = 1, Nome = "Nintendo" });

        _condicaoRepositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<CondicaoJogo>());

        var service = CriarJogoService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.InserirAsync(jogo, idsGeneros)
        );

        // Assert
        Assert.Equal(BusinessErrors.CONDICAO_NAO_ENCONTRADA, excecao.Erro);

        _jogoRepositoryMock.Verify(
            r => r.InserirAsync(It.IsAny<Jogo>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Inserir_GeneroInexistente_LancaGeneroNaoEncontrado()
    {
        // Arrange
        var jogo = CriarJogo();
        var idsGeneros = new List<uint> { 1 };

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(jogo.IdPlataforma))
            .ReturnsAsync(new Plataforma { Id = 1, Nome = "Nintendo" });

        _condicaoRepositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<CondicaoJogo>
            {
                new() { Id = 1, Descricao = "Novo" }
            });

        _generoRepositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<Genero>());

        var service = CriarJogoService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.InserirAsync(jogo, idsGeneros)
        );

        // Assert
        Assert.Equal(BusinessErrors.GENERO_NAO_ENCONTRADO, excecao.Erro);

        _jogoRepositoryMock.Verify(
            r => r.InserirAsync(It.IsAny<Jogo>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Listar_ComGenero_EnviaIdsDosJogosDoGeneroAoRepository()
    {
        // Arrange
        var vinculos = new List<(uint IdJogo, uint IdGenero)>
        {
            (10, 2),
            (20, 2),
            (30, 3)
        };

        _jogoGeneroRepositoryMock
            .Setup(r => r.ListarTodosAsync())
            .ReturnsAsync(vinculos);

        _jogoRepositoryMock
            .Setup(r => r.ListarAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string?>(),
                It.IsAny<uint?>(),
                It.IsAny<List<uint>?>(),
                It.IsAny<bool>()
            ))
            .ReturnsAsync((new List<Jogo>(), 0));

        var service = CriarJogoService();

        // Act
        await service.ListarAsync(
            pagina: 1,
            tamanhoPagina: 10,
            idGenero: 2
        );

        // Assert
        _jogoRepositoryMock.Verify(
            r => r.ListarAsync(
                1,
                10,
                null,
                null,
                It.Is<List<uint>?>(ids =>
                    ids != null &&
                    ids.SequenceEqual(new List<uint> { 10, 20 })
                ),
                false
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task Listar_SemGenero_NaoConsultaVinculosENaoRestringeIds()
    {
        // Arrange
        _jogoRepositoryMock
            .Setup(r => r.ListarAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string?>(),
                It.IsAny<uint?>(),
                It.IsAny<List<uint>?>(),
                It.IsAny<bool>()
            ))
            .ReturnsAsync((new List<Jogo>(), 0));

        var service = CriarJogoService();

        // Act
        await service.ListarAsync(
            pagina: 1,
            tamanhoPagina: 10
        );

        // Assert
        _jogoGeneroRepositoryMock.Verify(
            r => r.ListarTodosAsync(),
            Times.Never
        );

        _jogoRepositoryMock.Verify(
            r => r.ListarAsync(
                1,
                10,
                null,
                null,
                null,
                false
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task Listar_ComJogos_MontaNomeDaPlataformaEDescricaoDaCondicao()
    {
        // Arrange
        var jogo = CriarJogo();
        jogo.Id = 10;

        _jogoRepositoryMock
            .Setup(r => r.ListarAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string?>(),
                It.IsAny<uint?>(),
                It.IsAny<List<uint>?>(),
                It.IsAny<bool>()
            ))
            .ReturnsAsync((new List<Jogo> { jogo }, 1));

        _plataformaRepositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<Plataforma>
            {
                new() { Id = 1, Nome = "Nintendo" }
            });

        _condicaoRepositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<CondicaoJogo>
            {
                new() { Id = 1, Descricao = "Novo" }
            });

        _generoRepositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<Genero>());

        _jogoGeneroRepositoryMock
            .Setup(r => r.ListarPorJogosAsync(It.IsAny<List<uint>>()))
            .ReturnsAsync(new List<(uint IdJogo, uint IdGenero)>());

        var service = CriarJogoService();

        // Act
        var (jogos, total) = await service.ListarAsync(
            pagina: 1,
            tamanhoPagina: 10
        );

        // Assert
        Assert.Equal(1, total);
        Assert.Single(jogos);
        Assert.Equal("Nintendo", jogos[0].NomePlataforma);
        Assert.Equal("Novo", jogos[0].DescricaoCondicao);
    }

    [Fact]
    public async Task BuscarPorId_IdInexistente_LancaJogoNaoEncontrado()
    {
        // Arrange
        _jogoRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(999))
            .ReturnsAsync((Jogo?)null);

        var service = CriarJogoService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.BuscarPorIdAsync(999)
        );

        // Assert
        Assert.Equal(BusinessErrors.JOGO_NAO_ENCONTRADO, excecao.Erro);
    }

    [Fact]
    public async Task Desativar_IdInexistente_LancaJogoNaoEncontradoENaoDesativa()
    {
        // Arrange
        _jogoRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(999))
            .ReturnsAsync((Jogo?)null);

        var service = CriarJogoService();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.DesativarAsync(999)
        );

        // Assert
        Assert.Equal(BusinessErrors.JOGO_NAO_ENCONTRADO, excecao.Erro);

        _jogoRepositoryMock.Verify(
            r => r.DesativarAsync(999),
            Times.Never
        );
    }
}