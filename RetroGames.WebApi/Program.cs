using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Middlewares;
using DotNetEnv;

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

// Registra os controllers.
builder.Services.AddControllers();

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