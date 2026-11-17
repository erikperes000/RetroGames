using RetroGames.Services.Interfaces;
using RetroGames.Services.Services;
using Microsoft.Extensions.DependencyInjection;
using RetroGames.Repository;
using RetroGames.Repository.Config;
using RetroGames.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Model;

namespace RetroGames.WebApi.Configuration;

public static class InjecaoDependenciaConfig{
    public static IServiceCollection AddRepositorios(this IServiceCollection services, string connectionString){
        services.AddSingleton(
            new RepositoryConfig(connectionString));

        services.AddScoped<IRepositorySet, RepositorySet>();

        services.AddScoped<IPlataformaRepository>(
            provider => provider.GetRequiredService<IRepositorySet>().Plataforma);

        services.AddScoped<IJogoRepository>(
            provider => provider.GetRequiredService<IRepositorySet>().Jogo);

        services.AddScoped<IJogoGeneroRepository>(
            provider => provider.GetRequiredService<IRepositorySet>().JogoGenero);

        services.AddScoped<IGeneroRepository>(
            provider => provider.GetRequiredService<IRepositorySet>().Genero);

        services.AddScoped<ICondicaoJogoRepository>(
            provider => provider.GetRequiredService<IRepositorySet>().CondicaoJogo);

        services.AddScoped<IGeneroService, GeneroService>();
        services.AddScoped<ICondicaoJogoService, CondicaoJogoService>();

        services.AddScoped<IUsuarioRepository>(
            provider => provider.GetRequiredService<IRepositorySet>().Usuario);

        return services;
    }

    public static IServiceCollection AddServicosDeNegocio(this IServiceCollection services){
        services.AddScoped<IPlataformaService, PlataformaService>();
        services.AddScoped<IJogoService, JogoService>();

        // PasswordHasher responsável pelo hash e verificação de senhas.
        services.AddScoped<IPasswordHasher<Usuario>,
            PasswordHasher<Usuario>>();

        // Serviço de autenticação.
        services.AddScoped<IAutenticacaoService, AutenticacaoService>();

        return services;
    }
}
