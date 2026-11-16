using FluentValidation;
using RetroGames.WebApi.ApiModels.Requests;

namespace RetroGames.WebApi.Validators.Jogo;

public class InserirJogoRequestValidator
    : AbstractValidator<InserirJogoRequestApiModel>
{
    public InserirJogoRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("Informe o nome do jogo.")
            .MaximumLength(150)
            .WithMessage("O nome deve ter até 150 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000)
            .WithMessage("A descrição deve ter até 1000 caracteres.");

        RuleFor(x => x.AnoLancamento)
            .InclusiveBetween((ushort)1970, (ushort)2100)
            .WithMessage("Ano de lançamento inválido.");

        RuleFor(x => x.Preco)
            .GreaterThan(0)
            .WithMessage("O preço deve ser maior que zero.")
            .Must(preco =>
                decimal.Round(preco, 2) == preco)
            .WithMessage("O preço deve ter até duas casas decimais.");

        RuleFor(x => x.IdPlataforma)
            .Must(id => id > 0)
            .WithMessage("A plataforma é obrigatória.");

        RuleFor(x => x.IdCondicaoJogo)
            .GreaterThan((ushort)0)
            .WithMessage("Informe a condição do jogo.");

        RuleFor(x => x.IdsGeneros)
            .NotNull()
            .WithMessage("Informe ao menos um gênero.")
            .Must(ids => ids != null && ids.Count > 0)
            .WithMessage("Informe ao menos um gênero.");
    }
}