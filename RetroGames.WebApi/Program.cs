using DotNetEnv;
using FluentValidation;
using RetroGames.WebApi.Validators.Jogo;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using RetroGames.WebApi.ActionFilters;
using RetroGames.WebApi.ApiModels.Constantes;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Middlewares;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.WebApi.ApiModels.Responses;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Obtém a connection string do arquivo .env.
var connectionString =
    builder.Configuration.GetConnectionString("Default");

// Interrompe a aplicação se a connection string não existir.
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "A connection string 'ConnectionStrings:Default' não foi encontrada. " +
        "Verifique se o arquivo .env existe e contém ConnectionStrings__Default."
    );
}

// Registra os repositórios e os Services.
builder.Services.AddRepositorios(connectionString);

// Registra automaticamente os validators do projeto.
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Registra o filtro de validação.
builder.Services.AddScoped<FluentValidationFilter>();

// Registra os controllers e o filtro global.
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var erros = context.ModelState.Values
                .SelectMany(valor => valor.Errors);

            var jsonMalformado = erros.Any(erro =>
                erro.Exception is JsonException ||
                erro.Exception?.InnerException is JsonException);

            if (jsonMalformado)
            {
                return new BadRequestObjectResult(new
                {
                    responseCode = 3,
                    errorMessage = "JSON inválido ou malformado."
                });
            }

            var errosPorCampo = context.ModelState
                .Where(item => item.Value != null)
                .Where(item => item.Value!.Errors.Count > 0)
                .ToDictionary(
                    item => JsonNamingPolicy.CamelCase.ConvertName(item.Key),
                    item => item.Value!.Errors
                        .Select(erro =>
                            string.IsNullOrWhiteSpace(erro.ErrorMessage)
                                ? "Valor inválido."
                                : erro.ErrorMessage)
                        .ToArray());

            return new BadRequestObjectResult(new
            {
                responseCode = 3,
                data = errosPorCampo,
                errorMessage = "Há um erro com o modelo enviado."
            });
        };
    });

builder.Services.ConfigureHttpJsonOptions(opcoes =>
{
    opcoes.SerializerOptions.NumberHandling =
        JsonNumberHandling.Strict;
});

// Configura o tratamento automático de erros de ModelState.
builder.Services.Configure<ApiBehaviorOptions>(opcoes =>
{
    opcoes.InvalidModelStateResponseFactory = contexto =>
    {
        // Verifica se o erro foi causado por um JSON malformado.
        var jsonMalformado = contexto.ModelState.Values
            .SelectMany(valor => valor.Errors)
            .Any(erro =>
                erro.Exception is JsonException ||
                erro.Exception?.InnerException is JsonException);

        // JSON malformado: não apresenta lista de campos.
        if (jsonMalformado)
        {
            return new BadRequestObjectResult(new
            {
                responseCode = ResponseCodes.MODEL_STATE_INVALIDO,
                errorMessage = "Há um erro com o modelo enviado."
            });
        }

        // Organiza os erros por campo em camelCase.
        var errosPorCampo = contexto.ModelState
            .Where(item => item.Value != null)
            .Where(item => item.Value!.Errors.Count > 0)
            .ToDictionary(
                item => JsonNamingPolicy.CamelCase.ConvertName(
                    item.Key),
                item => item.Value!.Errors
                    .Select(erro =>
                        string.IsNullOrWhiteSpace(erro.ErrorMessage)
                            ? "Valor inválido."
                            : erro.ErrorMessage)
                    .ToArray());

        return new BadRequestObjectResult(new
        {
            responseCode = ResponseCodes.MODEL_STATE_INVALIDO,
            data = errosPorCampo,
            errorMessage = "Há um erro com o modelo enviado."
        });
    };
});

// Registra o OpenAPI.
builder.Services.AddOpenApi();

builder.Services.AddServicosDeNegocio();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

// Configura o Swagger somente em ambiente de desenvolvimento.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "RetroGames API v1"
        );
    });
}

// Mapeia os controllers.
app.MapControllers();

app.Run();