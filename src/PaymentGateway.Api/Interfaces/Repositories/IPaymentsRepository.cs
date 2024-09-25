using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Interfaces.Repositories
{
    public interface IPaymentsRepository
    {
        Task<PostPaymentResponse?> GetPaymentByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task SaveAsync(PostPaymentResponse payment);
    }
}
