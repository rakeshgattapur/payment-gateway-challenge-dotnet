using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

using Refit;

namespace PaymentGateway.Api.ServiceClients;

public interface IAcquiringBankServiceClient
{
    [Post("/payments")]
    Task<AcquiringBankResponse> AuthorizePaymentAsync([Body] AcquiringBankRequest acquiringBankRequest, CancellationToken cancellationToken = default);
}
