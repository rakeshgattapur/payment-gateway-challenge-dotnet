using FluentValidation;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Extensions;
using PaymentGateway.Api.Interfaces.Services;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly IPaymentsService _paymentsService;
    private readonly IValidator<PostPaymentRequest> _paymentValidator;

    public PaymentsController(IPaymentsService paymentsService,
        IValidator<PostPaymentRequest> paymentValidator)
    {
        _paymentsService = paymentsService;
        _paymentValidator = paymentValidator;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = await _paymentsService.GetPaymentByIdAsync(id);

        return payment switch
        {
            null => NotFound("The payment you requested does not exist"),
            _ => new OkObjectResult(payment)
        };
    }

    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse?>> ProcessPaymentAsync(PostPaymentRequest postPaymentRequest, CancellationToken cancellationToken = default)
    {
        var validationResult = await _paymentValidator.ValidateAsync(postPaymentRequest, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToPostPaymentBadRequestResponse();
        }

        var postPaymentResponse = await _paymentsService.ProcessPaymentAsync(postPaymentRequest, cancellationToken);

        return postPaymentResponse switch
        {
            null => StatusCode(500, "An unexpected error occurred. Please try again later."),
            _ => postPaymentResponse.Status switch
            {
                Enums.PaymentStatus.Rejected => new BadRequestObjectResult(new PostPaymentValidationResponse { Errors = ["Invalid Payment."] }),
                _ => new OkObjectResult(postPaymentResponse)
            }
        };
    }
}