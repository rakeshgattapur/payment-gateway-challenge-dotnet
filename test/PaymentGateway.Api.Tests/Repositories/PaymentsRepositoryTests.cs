using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Repositories;

namespace PaymentGateway.Api.Tests.Repositories;

public class InMemoryPaymentsRepositoryTests
{
    private readonly InMemoryPaymentsRepository _repository;

    public InMemoryPaymentsRepositoryTests()
    {
        _repository = new InMemoryPaymentsRepository();
    }

    [Fact]
    public async Task SaveAsync_Should_Add_New_Payment()
    {
        // Arrange
        var payment = new PostPaymentResponse
        {
            Id = Guid.NewGuid(),
            Amount = 100,
            Currency = "USD",
            ExpiryMonth = 12,
            ExpiryYear = 2025,
            Status = Enums.PaymentStatus.Authorized,
            CardNumberLastFourDigits = "1234"
        };

        // Act
        await _repository.SaveAsync(payment);

        // Assert
        var retrievedPayment = await _repository.GetPaymentByIdAsync(payment.Id);
        Assert.NotNull(retrievedPayment);
        Assert.Equal(payment, retrievedPayment);
    }

    [Fact]
    public async Task GetPaymentByIdAsync_Should_Return_Payment_When_Exists()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var payment = new PostPaymentResponse
        {
            Id = paymentId,
            Amount = 100,
            Currency = "USD",
            ExpiryMonth = 12,
            ExpiryYear = 2025,
            Status = Enums.PaymentStatus.Authorized,
            CardNumberLastFourDigits = "1234"
        };

        await _repository.SaveAsync(payment);

        // Act
        var result = await _repository.GetPaymentByIdAsync(paymentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(paymentId, result?.Id);
        Assert.Equal(payment, result);
    }

    [Fact]
    public async Task GetPaymentByIdAsync_Should_Return_Null_When_Not_Found()
    {
        // Arrange
        var nonExistentPaymentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetPaymentByIdAsync(nonExistentPaymentId);

        // Assert
        Assert.Null(result);
    }
}
