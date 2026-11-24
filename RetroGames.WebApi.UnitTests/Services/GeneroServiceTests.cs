using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class GeneroServiceTests
{
    [Fact]
    public async Task Listar_DevolveGenerosRetornadosPeloRepository()
    {
        // Arrange
        var generosEsperados = new List<Genero>
        {
            new() { Id = 1, Nome = "Ação" },
            new() { Id = 2, Nome = "Aventura" }
        };

        var generoRepositoryMock = new Mock<IGeneroRepository>();
        generoRepositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(generosEsperados);

        var repositorioMock = new Mock<IRepositorySet>();
        repositorioMock
            .SetupGet(r => r.Genero)
            .Returns(generoRepositoryMock.Object);

        var service = new GeneroService(repositorioMock.Object);

        // Act
        var resultado = await service.ListarAsync();

        // Assert
        Assert.Same(generosEsperados, resultado);

        generoRepositoryMock.Verify(
            r => r.ListarAsync(),
            Times.Once
        );
    }
}