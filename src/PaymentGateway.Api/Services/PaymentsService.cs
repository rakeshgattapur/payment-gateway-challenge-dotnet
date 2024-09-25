using System.Net;
using System.Text.Json;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Extensions;
using PaymentGateway.Api.Interfaces.Repositories;
using PaymentGateway.Api.Interfaces.Services;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.ServiceClients;

using Refit;
namespace PaymentGateway.Api.Services;

public class PaymentsService : IPaymentsService
{
    private readonly ILogger<PaymentsService> _logger;
    private readonly IPaymentsRepository _paymentsRepository;
    private readonly IAcquiringBankServiceClient _acquiringBankServiceClient;
    public PaymentsService(ILogger<PaymentsService> logger, IPaymentsRepository paymentsRepository, IAcquiringBankServiceClient acquiringBankServiceClient)
    {
        _logger = logger;
        _paymentsRepository = paymentsRepository;
        _acquiringBankServiceClient = acquiringBankServiceClient;
    }

    public async Task<PostPaymentResponse?> GetPaymentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("PaymentsService.GetPaymentByIdAsync - get payment with id: {Id}", id);
        var paymentResponse = await _paymentsRepository.GetPaymentByIdAsync(id, cancellationToken);
        return paymentResponse;
    }

    public async Task<PostPaymentResponse?> ProcessPaymentAsync(PostPaymentRequest paymentRequest, CancellationToken cancellationToken = default)
    {
        //preferably the request should be obfuscated in production
        _logger.LogInformation("PaymentsService.ProcessPaymentAsync - payment request: {request}", JsonSerializer.Serialize(paymentRequest));

        var acquiringBankRequest = new AcquiringBankRequest(paymentRequest.CardNumber, $"{paymentRequest.ExpiryMonth:D2}/{paymentRequest.ExpiryYear}",
            paymentRequest.Currency, paymentRequest.Amount, paymentRequest.Cvv.ToString());

        AcquiringBankResponse? acquiringBankResponse;

        try
        {
            acquiringBankResponse = await _acquiringBankServiceClient.AuthorizePaymentAsync(acquiringBankRequest, cancellationToken);
        }
        catch (ApiException exception)
        {
            _logger.LogCritical(exception, "An exception occured while calling acquiring bank service, statuscode: {StatusCode}", exception.StatusCode);

            return exception.StatusCode switch
            {
                HttpStatusCode.BadRequest => new PostPaymentResponse { Status = PaymentStatus.Rejected },
                _ => null
            };
        }

        if (acquiringBankRequest == null)
            return null;

        var postPaymentResponse = new PostPaymentResponse()
        {
            Amount = paymentRequest.Amount,
            CardNumberLastFourDigits = paymentRequest.CardNumber.GetLastChars(4),
            Currency = paymentRequest.Currency,
            ExpiryMonth = paymentRequest.ExpiryMonth,
            ExpiryYear = paymentRequest.ExpiryYear,
            Id = Guid.NewGuid(),
            Status = acquiringBankResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined,
        };

        await _paymentsRepository.SaveAsync(postPaymentResponse);

        return postPaymentResponse;
    }
}
