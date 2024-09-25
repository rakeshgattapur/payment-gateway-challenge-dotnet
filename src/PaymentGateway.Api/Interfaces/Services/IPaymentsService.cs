using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Interfaces.Services;

public interface IPaymentsService
{
    /// <summary>
    /// Gets a payment by provided Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>PostPaymentResponse || null</returns>
    Task<PostPaymentResponse?> GetPaymentByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Process a payment
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>PostPaymentResponse || null</returns>
    Task<PostPaymentResponse?> ProcessPaymentAsync(PostPaymentRequest request, CancellationToken cancellationToken = default);
}
