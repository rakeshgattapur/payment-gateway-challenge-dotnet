using System.Collections.Concurrent;

using PaymentGateway.Api.Interfaces.Repositories;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Repositories;

public class InMemoryPaymentsRepository : IPaymentsRepository
{
    private readonly ConcurrentDictionary<Guid, PostPaymentResponse> _payments = new();

    public Task SaveAsync(PostPaymentResponse payment)
    {
        _payments[payment.Id] = payment;
        return Task.CompletedTask;
    }

    public Task<PostPaymentResponse?> GetPaymentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _payments.TryGetValue(id, out var payment);
        return Task.FromResult(payment);
    }
}
