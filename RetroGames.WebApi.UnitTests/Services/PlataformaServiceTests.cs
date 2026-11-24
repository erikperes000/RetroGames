using Moq;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;
using RetroGames.Services.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class PlataformaServiceTests
{
    // Arrange: criação dos mocks
    private readonly Mock<IPlataformaRepository> _plataformaRepositoryMock;
    private readonly Mock<IRepositorySet> _repositorioMock;

    public PlataformaServiceTests()
    {
        _plataformaRepositoryMock = new Mock<IPlataformaRepository>();
        _repositorioMock = new Mock<IRepositorySet>();

        // O RepositorySet devolve o mock de Plataforma
        _repositorioMock
            .SetupGet(r => r.Plataforma)
            .Returns(_plataformaRepositoryMock.Object);
    }

    // Cria o Service utilizando os repositórios simulados
    private PlataformaService CriarPlataformaService()
    {
        return new PlataformaService(_repositorioMock.Object);
    }

    [Fact]
    public async Task BuscarPorId_IdInexistente_LancaNaoEncontrada()
    {
        // Arrange
        var service = CriarPlataformaService();
        uint idInexistente = 999;

        _plataformaRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(idInexistente))
            .ReturnsAsync((RetroGames.Model.Model.Plataforma?)null);

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
}