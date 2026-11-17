using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.WebApi.ApiModels.Constantes;
using RetroGames.WebApi.ApiModels.Responses;

namespace RetroGames.WebApi.Configuration;

public static class AutenticacaoConfig
{
    public static IServiceCollection AddAutenticacao(
        this IServiceCollection services)
    {
        services
            .AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opcoes =>
            {
                opcoes.Cookie.Name = "retrogames.sessao";
                opcoes.Cookie.HttpOnly = true;
                opcoes.Cookie.SameSite = SameSiteMode.Strict;
                opcoes.Cookie.SecurePolicy =
                    CookieSecurePolicy.SameAsRequest;

                opcoes.ExpireTimeSpan = TimeSpan.FromHours(2);
                opcoes.SlidingExpiration = true;

                opcoes.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = contexto =>
                    {
                        contexto.Response.StatusCode =
                            StatusCodes.Status401Unauthorized;

                        var resposta = new RetroGamesResponseApiModel
                        {
                            ResponseCode = ResponseCodes.NAO_AUTENTICADO,
                            ErrorMessage = "Faça login para continuar."
                        };

                        return contexto.Response.WriteAsJsonAsync(resposta);
                    },

                    OnRedirectToAccessDenied = contexto =>
                    {
                        contexto.Response.StatusCode =
                            StatusCodes.Status403Forbidden;

                        var resposta = new RetroGamesResponseApiModel
                        {
                            ResponseCode = ResponseCodes.ACESSO_NEGADO,
                            ErrorMessage =
                                "Você não tem permissão para esta operação."
                        };

                        return contexto.Response.WriteAsJsonAsync(resposta);
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}