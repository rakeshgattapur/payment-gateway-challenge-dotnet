using FluentValidation;

using PaymentGateway.Api.Interfaces.Services;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Validators;

public class PostPaymentRequestValidator : AbstractValidator<PostPaymentRequest>
{
    private readonly ICurrencyService _currencyService;
    public PostPaymentRequestValidator(ICurrencyService currencyService)
    {
        _currencyService = currencyService;

        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("Card number is required.")
            .Must(BeAValidNumber).WithMessage("Card number must contain only numeric values")
            .Length(14, 19).WithMessage("Card number must be between 14 and 19 digits long.");

        RuleFor(x => x.ExpiryMonth)
            .NotEmpty().WithMessage("Expiry month is required.")
            .InclusiveBetween(1, 12).WithMessage("Expiry month must be between 1 and 12.");

        RuleFor(x => x.ExpiryYear)
            .NotEmpty().WithMessage("Expiry year is required.")
            .Must(BeAValidExpiryDate).WithMessage("Card expiry date must be in the future.");

        RuleFor(x => x.Cvv)
        .NotEmpty().WithMessage("Cvv is required.")
        .Must(cvv => BeAValidLengthNumber(cvv.ToString(), 3, 4)).WithMessage("Cvv must be 3 or 4 digits long.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Length(3).WithMessage("Currency should be 3 characters long valid ISO code")
            .Must(BeAValidCurrency).WithMessage("Currency code must be valid ISO code, for example: USD, EUR, GBP.");

        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("Amount is required.")
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");
    }

    private bool BeAValidExpiryDate(PostPaymentRequest request, int expiryYear)
    {
        var currentYear = DateTime.Now.Year;
        var currentMonth = DateTime.Now.Month;

        return expiryYear > currentYear || expiryYear == currentYear && request.ExpiryMonth >= currentMonth;
    }

    private bool BeAValidCurrency(string currency)
    {
        var allowedCurrencies = _currencyService.GetValidCurrencies().ToArray();
        return Array.Exists(allowedCurrencies, c => c.Equals(currency, StringComparison.OrdinalIgnoreCase));
    }

    private static bool BeAValidNumber(string number)
    {
        string cardNumberStr = number.ToString();
        return cardNumberStr.All(char.IsDigit);
    }

    private static bool BeAValidLengthNumber(string numberString, int minLength, int maxLength)
    {
        return numberString.Length >= minLength && numberString.Length <= maxLength;
    }
}
