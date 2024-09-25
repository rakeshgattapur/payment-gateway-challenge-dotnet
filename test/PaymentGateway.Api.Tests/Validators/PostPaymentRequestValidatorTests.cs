using FluentValidation.TestHelper;

using Moq;

using PaymentGateway.Api.Interfaces.Services;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Validators;

namespace PaymentGateway.Api.Tests.Validators;

public class PostPaymentRequestValidatorTests
{
    private readonly PostPaymentRequestValidator _validator;
    private readonly Mock<ICurrencyService> _currencyServiceMock;

    public PostPaymentRequestValidatorTests()
    {
        _currencyServiceMock = new Mock<ICurrencyService>();
        _currencyServiceMock.Setup(service => service.GetValidCurrencies())
            .Returns(new List<string> { "USD", "EUR", "GBP" });

        _validator = new PostPaymentRequestValidator(_currencyServiceMock.Object);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_CardNumber_Is_Empty()
    {
        // Arrange
        var request = new PostPaymentRequest { CardNumber = "" };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.CardNumber)
            .WithErrorMessage("Card number is required.");
    }

    [Fact]
    public void Validator_Should_Have_Error_When_CardNumber_Is_Not_Numeric()
    {
        // Arrange
        var request = new PostPaymentRequest { CardNumber = "1234abcd5678" };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.CardNumber)
            .WithErrorMessage("Card number must contain only numeric values");
    }

    [Fact]
    public void Validator_Should_Have_Error_When_CardNumber_Length_Is_Invalid()
    {
        // Arrange
        var request = new PostPaymentRequest { CardNumber = "123456789012" };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.CardNumber)
            .WithErrorMessage("Card number must be between 14 and 19 digits long.");
    }

    [Fact]
    public void Validator_Should_Have_Error_When_ExpiryMonth_Is_Invalid()
    {
        // Arrange
        var request = new PostPaymentRequest { ExpiryMonth = 13 };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ExpiryMonth)
            .WithErrorMessage("Expiry month must be between 1 and 12.");
    }

    [Fact]
    public void Validator_Should_Have_Error_When_ExpiryYear_Is_In_The_Past()
    {
        // Arrange
        var request = new PostPaymentRequest { ExpiryMonth = 5, ExpiryYear = DateTime.Now.Year - 1 };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ExpiryYear)
            .WithErrorMessage("Card expiry date must be in the future.");
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Cvv_Is_Invalid_Length()
    {
        // Arrange
        var request = new PostPaymentRequest { Cvv = 12 };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Cvv)
            .WithErrorMessage("Cvv must be 3 or 4 digits long.");
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Currency_Is_Invalid()
    {
        // Arrange
        var request = new PostPaymentRequest { Currency = "ABC" };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Currency)
            .WithErrorMessage("Currency code must be valid ISO code, for example: USD, EUR, GBP.");
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Amount_Is_Less_Than_Or_Equal_To_Zero()
    {
        // Arrange
        var request = new PostPaymentRequest { Amount = 0 };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage("Amount must be greater than zero.");
    }

    [Fact]
    public void Validator_Should_Pass_For_Valid_Request()
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = "1234567890123456",
            ExpiryMonth = 12,
            ExpiryYear = DateTime.Now.Year + 1,
            Cvv = 123,
            Currency = "USD",
            Amount = 100
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
