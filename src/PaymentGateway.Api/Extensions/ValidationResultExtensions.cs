using FluentValidation.Results;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Extensions
{
    public static class ValidationResultExtensions
    {
        public static BadRequestObjectResult ToPostPaymentBadRequestResponse(
            this ValidationResult validationResult)
        {
            var errors = validationResult.Errors
                .Select(error => error.ErrorMessage)
                .ToList();

            return new BadRequestObjectResult(new PostPaymentValidationResponse { Errors = errors });
        }
    }
}
