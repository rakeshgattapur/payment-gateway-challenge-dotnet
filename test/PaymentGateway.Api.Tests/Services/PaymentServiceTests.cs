using Microsoft.Extensions.Logging;

using Moq;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Interfaces.Repositories;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.ServiceClients;
using PaymentGateway.Api.Services;

using Refit;

namespace PaymentGateway.Api.Tests.Services;

public class PaymentsServiceTests
{
    private readonly PaymentsService _paymentsService;
    private readonly Mock<ILogger<PaymentsService>> _loggerMock;
    private readonly Mock<IPaymentsRepository> _paymentsRepositoryMock;
    private readonly Mock<IAcquiringBankServiceClient> _acquiringBankServiceClientMock;

    public PaymentsServiceTests()
    {
        _loggerMock = new Mock<ILogger<PaymentsService>>();
        _paymentsRepositoryMock = new Mock<IPaymentsRepository>();
        _acquiringBankServiceClientMock = new Mock<IAcquiringBankServiceClient>();

        _paymentsService = new PaymentsService(_loggerMock.Object, _paymentsRepositoryMock.Object, _acquiringBankServiceClientMock.Object);
    }

    [Fact]
    public async Task GetPaymentByIdAsync_Should_Return_Payment_When_Found()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var expectedPaymentResponse = new PostPaymentResponse { Id = paymentId };
        _paymentsRepositoryMock.Setup(repo => repo.GetPaymentByIdAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPaymentResponse);

        // Act
        var result = await _paymentsService.GetPaymentByIdAsync(paymentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(paymentId, result?.Id);
        _paymentsRepositoryMock.Verify(repo => repo.GetPaymentByIdAsync(paymentId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessPaymentAsync_Should_Process_And_Save_Payment_When_Authorized()
    {
        // Arrange
        var paymentRequest = new PostPaymentRequest
        {
            CardNumber = "1234567812345678",
            ExpiryMonth = 12,
            ExpiryYear = DateTime.Now.Year + 1,
            Cvv = 123,
            Currency = "USD",
            Amount = 100
        };

        var acquiringBankResponse = new AcquiringBankResponse { Authorized = true };
        _acquiringBankServiceClientMock.Setup(client => client.AuthorizePaymentAsync(It.IsAny<AcquiringBankRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(acquiringBankResponse);

        // Act
        var result = await _paymentsService.ProcessPaymentAsync(paymentRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PaymentStatus.Authorized, result?.Status);
        _paymentsRepositoryMock.Verify(repo => repo.SaveAsync(It.IsAny<PostPaymentResponse>()), Times.Once);
    }

    [Fact]
    public async Task ProcessPaymentAsync_Should_Not_Call_Repo_And_Return_Null_On_ApiException()
    {
        // Arrange
        var paymentRequest = new PostPaymentRequest();
        var refitSettings = new RefitSettings();

        var apiException = ApiException.Create(null, null,
            new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.NotFound,
                Content = refitSettings.ContentSerializer.ToHttpContent(string.Empty)
            }, refitSettings).Result;

        _acquiringBankServiceClientMock.Setup(client => client.AuthorizePaymentAsync(It.IsAny<AcquiringBankRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        // Act
        var result = await _paymentsService.ProcessPaymentAsync(paymentRequest);

        // Assert
        Assert.Null(result);
        _paymentsRepositoryMock.Verify(repo => repo.SaveAsync(It.IsAny<PostPaymentResponse>()), Times.Never);
    }
}
