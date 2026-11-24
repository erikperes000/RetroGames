
using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository.Exceptions;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;
using RetroGames.Services.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class PlataformaServiceTests
{
    private readonly Mock<IPlataformaRepository> _plataformaRepositoryMock;
    private readonly Mock<IRepositorySet> _repositorioMock;

    public PlataformaServiceTests()
    {
        _plataformaRepositoryMock = new Mock<IPlataformaRepository>();
        _repositorioMock = new Mock<IRepositorySet>();

        _repositorioMock
            .SetupGet(r => r.Plataforma)
            .Returns(_plataformaRepositoryMock.Object);
    }

    private PlataformaService CriarPlataformaService()
    {
        return new PlataformaService(_repositorioMock.Object);
    }

    // TESTE 1: Buscar plataforma inexistente

    [Fact]
    public async Task BuscarPorId_IdInexistente_LancaNaoEncontrada()
    {
        // Arrange
        var service = CriarPlataformaService();
        uint idInexistente = 999;

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(idInexistente))
            .ReturnsAsync((Plataforma?)null);

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.BuscarPorIdAsync(idInexistente)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.PLATAFORMA_NAO_ENCONTRADA,
            excecao.Erro
        );
    }

    // TESTE 2: Inserir plataforma com nome duplicado

    [Fact]
    public async Task Inserir_NomeJaCadastrado_LancaNomeDuplicado()
    {
        // Arrange
        var service = CriarPlataformaService();

        var plataforma = new Plataforma
        {
            Nome = "PlayStation",
            Fabricante = "Sony",
            AnoLancamento = 1994
        };

        _plataformaRepositoryMock
            .Setup(r => r.ExisteComNomeAsync("PlayStation", null))
            .ReturnsAsync(true);

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.InserirAsync(plataforma)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.PLATAFORMA_NOME_DUPLICADO,
            excecao.Erro
        );

        _plataformaRepositoryMock.Verify(
            r => r.InserirAsync(It.IsAny<Plataforma>()),
            Times.Never
        );
    }

    // TESTE 3: Repository lança RegistroDuplicadoException

    [Fact]
    public async Task Inserir_RepositoryLancaRegistroDuplicado_LancaNomeDuplicado()
    {
        // Arrange
        var service = CriarPlataformaService();

        var plataforma = new Plataforma
        {
            Nome = "Nintendo 64",
            Fabricante = "Nintendo",
            AnoLancamento = 1996
        };

        _plataformaRepositoryMock
            .Setup(r => r.ExisteComNomeAsync("Nintendo 64", null))
            .ReturnsAsync(false);

        _plataformaRepositoryMock
            .Setup(r => r.InserirAsync(plataforma))
            .ThrowsAsync(
                new RegistroDuplicadoException("Registro duplicado")
            );

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.InserirAsync(plataforma)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.PLATAFORMA_NOME_DUPLICADO,
            excecao.Erro
        );

        _plataformaRepositoryMock.Verify(
            r => r.InserirAsync(plataforma),
            Times.Once
        );
    }

    // TESTE 4: Inserir plataforma com nome livre

    [Fact]
    public async Task Inserir_NomeLivre_RetornaPlataformaComIdGerado()
    {
        // Arrange
        var service = CriarPlataformaService();

        var plataforma = new Plataforma
        {
            Nome = "Mega Drive",
            Fabricante = "Sega",
            AnoLancamento = 1988
        };

        uint idGerado = 10;

        _plataformaRepositoryMock
            .Setup(r => r.ExisteComNomeAsync("Mega Drive", null))
            .ReturnsAsync(false);

        _plataformaRepositoryMock
            .Setup(r => r.InserirAsync(plataforma))
            .ReturnsAsync(idGerado);

        // Act
        var resultado = await service.InserirAsync(plataforma);

        // Assert
        Assert.Equal(idGerado, resultado.Id);
        Assert.Equal("Mega Drive", resultado.Nome);

        _plataformaRepositoryMock.Verify(
            r => r.InserirAsync(plataforma),
            Times.Once
        );
    }

    // TESTE 5: Alterar mantendo o próprio nome

    [Fact]
    public async Task Alterar_MantendoProprioNome_RealizaAlteracao()
    {
        // Arrange
        var service = CriarPlataformaService();

        uint id = 1;

        var plataformaExistente = new Plataforma
        {
            Id = id,
            Nome = "PlayStation",
            Fabricante = "Sony",
            AnoLancamento = 1994
        };

        var plataformaAlterada = new Plataforma
        {
            Id = id,
            Nome = "PlayStation",
            Fabricante = "Sony",
            AnoLancamento = 1995
        };

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(id))
            .ReturnsAsync(plataformaExistente);

        _plataformaRepositoryMock
            .Setup(r => r.ExisteComNomeAsync("PlayStation", id))
            .ReturnsAsync(false);

        _plataformaRepositoryMock
            .Setup(r => r.AlterarAsync(plataformaAlterada))
            .Returns(Task.CompletedTask);

        // Act
        await service.AlterarAsync(plataformaAlterada);

        // Assert
        _plataformaRepositoryMock.Verify(
            r => r.AlterarAsync(plataformaAlterada),
            Times.Once
        );
    }

    // TESTE 6: Excluir plataforma que possui jogos

    [Fact]
    public async Task Excluir_PlataformaComJogos_LancaPossuiJogos()
    {
        // Arrange
        var service = CriarPlataformaService();

        uint id = 1;

        var plataforma = new Plataforma
        {
            Id = id,
            Nome = "PlayStation",
            Fabricante = "Sony",
            AnoLancamento = 1994
        };

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(id))
            .ReturnsAsync(plataforma);

        _plataformaRepositoryMock
            .Setup(r => r.PossuiJogosAsync(id))
            .ReturnsAsync(true);

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.ExcluirAsync(id)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.PLATAFORMA_POSSUI_JOGOS,
            excecao.Erro
        );

        _plataformaRepositoryMock.Verify(
            r => r.ExcluirAsync(id),
            Times.Never
        );
    }
}