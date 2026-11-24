using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class CondicaoJogoServiceTests
{
    [Fact]
    public async Task Listar_DevolveCondicoesRetornadasPeloRepository()
    {
        // Arrange
        var condicoesEsperadas = new List<CondicaoJogo>
        {
            new() { Id = 1, Descricao = "Novo" },
            new() { Id = 2, Descricao = "Usado" }
        };

        var condicaoRepositoryMock = new Mock<ICondicaoJogoRepository>();
        condicaoRepositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(condicoesEsperadas);

        var repositorioMock = new Mock<IRepositorySet>();
        repositorioMock
            .SetupGet(r => r.CondicaoJogo)
            .Returns(condicaoRepositoryMock.Object);

        var service = new CondicaoJogoService(repositorioMock.Object);

        // Act
        var resultado = await service.ListarAsync();

        // Assert
        Assert.Same(condicoesEsperadas, resultado);

        condicaoRepositoryMock.Verify(
            r => r.ListarAsync(),
            Times.Once
        );
    }
}