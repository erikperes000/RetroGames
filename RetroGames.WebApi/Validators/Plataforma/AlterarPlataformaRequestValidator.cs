using FluentValidation;
using RetroGames.WebApi.ApiModels.Requests;

namespace RetroGames.WebApi.Validators.Plataforma;

public class AlterarPlataformaRequestValidator
    : AbstractValidator<AlterarPlataformaRequestApiModel>
{
    public AlterarPlataformaRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("Informe o nome.")
            .MaximumLength(80)
            .WithMessage("O nome deve ter até 80 caracteres.");

        RuleFor(x => x.Fabricante)
            .NotEmpty()
            .WithMessage("Informe o fabricante.")
            .MaximumLength(80)
            .WithMessage("O fabricante deve ter até 80 caracteres.");

        RuleFor(x => x.AnoLancamento)
            .InclusiveBetween((ushort)1970, (ushort)2100)
            .WithMessage("Ano de lançamento inválido.");
    }
}