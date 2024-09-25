using AutoFixture;

using FluentValidation.Results;
using PaymentGateway.Api.Extensions;

using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Tests.Extensions;

public class ValidationResultExtensionsResult
{
    [Fact]
    public void ToPostPaymentBadRequestResponse_Returns_BadRequest_With_Status_And_Errors()
    {
        // Arrange
        var fixture = new Fixture();
        var errorMessages = fixture.CreateMany<string>(3).ToList();
        var validationFailures = errorMessages.Select(msg => new ValidationFailure("Property", msg)).ToList();
        var validationResult = new ValidationResult(validationFailures);

        // Act
        var result = validationResult.ToPostPaymentBadRequestResponse();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var responseObject = badRequestResult.Value as PostPaymentValidationResponse;
        Assert.Equal(Enums.PaymentStatus.Rejected, responseObject?.Status);
        Assert.Equal(errorMessages, responseObject?.Errors);
    }

    [Fact]
    public void ToPostPaymentBadRequestResponse_Returns_Empty_Errors_When_ValidationResult_Is_Valid()
    {
        // Arrange
        var validationResult = new ValidationResult();

        // Act
        var result = validationResult.ToPostPaymentBadRequestResponse();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var responseObject = badRequestResult.Value as PostPaymentValidationResponse;
        Assert.Empty(responseObject?.Errors); 
    }
}
