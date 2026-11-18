using Microsoft.AspNetCore.Authorization;
using RetroGames.Model.Constantes;

namespace RetroGames.WebApi.Configuration;

public static class AutorizacaoConfig
{
    public static IServiceCollection AddAutorizacao(
        this IServiceCollection services)
    {
        services.AddAuthorization(opcoes =>
        {
            // Por padrão, todas as rotas exigem autenticação.
            opcoes.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            // Política exclusiva para administradores.
            opcoes.AddPolicy(
                Politicas.Administrador,
                politica =>
                {
                    politica.RequireAuthenticatedUser();
                    politica.RequireRole(
                        nameof(Perfis.Administrador));
                });
        });

        return services;
    }
}