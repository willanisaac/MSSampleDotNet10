using BFF.Application.DTOs.Requests;
using FluentValidation;

namespace BFF.Application.Validators;

public class ProcessPaymentRequestValidator : AbstractValidator<ProcessPaymentRequest>
{
    public ProcessPaymentRequestValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0.");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("PaymentMethod is required.")
            .Must(x => new[] { "CreditCard", "DebitCard", "PayPal", "BankTransfer" }.Contains(x))
            .WithMessage("Invalid payment method.");
    }
}
