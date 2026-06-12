using FluentValidation;

namespace GestaoPedidosAPI.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("O CustomerId é obrigatório.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductName)
                .NotEmpty().WithMessage("O nome do produto não pode ser vazio.");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");

            item.RuleFor(i => i.UnitPrice)
                .GreaterThan(0).WithMessage("O preço unitário deve ser maior que zero.")
                .Must(v => decimal.Round(v, 2) == v).WithMessage("O preço unitário deve ser um valor monetário válido (máx. 2 casas decimais).");
        });
    }
}
