using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

using FluentValidation;
using FluentValidation.Results;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Interfaces.Repositories;
using PaymentGateway.Api.Interfaces.Services;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Tests.Utils;
using PaymentGateway.Api.Validators;

namespace PaymentGateway.Api.Tests.Controllers
{
    public class PaymentsControllerTests
    {
        private readonly WebApplicationFactory<PaymentsController> _factory;
        private readonly HttpClient _client;
        private readonly Mock<IPaymentsService> _paymentsService;
        private readonly Mock<IValidator<PostPaymentRequest>> _paymentValidator;

        public PaymentsControllerTests()
        {
            _paymentsService = new Mock<IPaymentsService>();
            _paymentValidator = new Mock<IValidator<PostPaymentRequest>>();

            _factory = new WebApplicationFactory<PaymentsController>()
                .WithWebHostBuilder(builder => builder.ConfigureServices(services =>
                    {
                        services.AddSingleton(_paymentsService.Object);
                        services.AddSingleton(_paymentValidator.Object);
                    }));

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetPayment_ReturnsPaymentSuccessfully()
        {
            // Arrange
            var payment = new PostPaymentResponse
            {
                Id = Guid.NewGuid(),
                Status = PaymentStatus.Authorized,
                ExpiryYear = 2030,
                ExpiryMonth = 12,
                Amount = 10000,
                CardNumberLastFourDigits = "1234",
                Currency = "GBP"
            };

            _paymentsService
                .Setup(x => x.GetPaymentByIdAsync(payment.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(payment);

            // Act
            var response = await _client.GetAsync($"/api/Payments/{payment.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>(Utilities.AddJsonSerializerOptions());
            Assert.NotNull(paymentResponse);
            Assert.Equal(payment.Id, paymentResponse?.Id);
            Assert.Equal(payment.Status, paymentResponse?.Status);
            Assert.Equal(payment.Amount, paymentResponse?.Amount);
            Assert.Equal(payment.Currency, paymentResponse?.Currency);

            _paymentsService.Verify(x => x.GetPaymentByIdAsync(payment.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetPayment_ReturnsNotFound_WhenPaymentDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/Payments/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ReturnsOk_WhenPaymentIsValid()
        {
            // Arrange
            var paymentRequest = new PostPaymentRequest
            {
                CardNumber = "1234567890123456",
                ExpiryMonth = 12,
                ExpiryYear = 2030,
                Cvv = 123,
                Amount = 100,
                Currency = "GBP"
            };

            var paymentResponse = new PostPaymentResponse
            {
                Id = Guid.NewGuid(),
                Status = PaymentStatus.Authorized,
                ExpiryYear = paymentRequest.ExpiryYear,
                ExpiryMonth = paymentRequest.ExpiryMonth,
                Amount = paymentRequest.Amount,
                CardNumberLastFourDigits = "3456",
                Currency = paymentRequest.Currency
            };

            var validationResult = new ValidationResult(); 

            _paymentValidator
                .Setup(x => x.ValidateAsync(It.IsAny<PostPaymentRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult); 

            _paymentsService
                .Setup(x => x.ProcessPaymentAsync(It.IsAny<PostPaymentRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paymentResponse);

            // Act
            var response = await _client.PostAsJsonAsync("/api/Payments", paymentRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var paymentResponseContent = await response.Content.ReadFromJsonAsync<PostPaymentResponse>(Utilities.AddJsonSerializerOptions());
            Assert.NotNull(paymentResponseContent);
            Assert.Equal(paymentResponse.Id, paymentResponseContent?.Id);
            Assert.Equal(paymentResponse.Status, paymentResponseContent?.Status);
            Assert.Equal(paymentResponse.Amount, paymentResponseContent?.Amount);
            Assert.Equal(paymentResponse.Currency, paymentResponseContent?.Currency);

            _paymentValidator.Verify(x => x.ValidateAsync(It.IsAny<PostPaymentRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            _paymentsService.Verify(x => x.ProcessPaymentAsync(It.IsAny<PostPaymentRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var paymentRequest = new PostPaymentRequest
            {
                CardNumber = "",
                ExpiryMonth = 0, 
                ExpiryYear = 2030,
                Cvv = 123,
                Amount = -100,
                Currency = "GBP"
            };

            var validationResult = new ValidationResult(new[]
            {
                new ValidationFailure(nameof(PostPaymentRequest.CardNumber), "Card number is required."),
                new ValidationFailure(nameof(PostPaymentRequest.ExpiryMonth), "Expiry month must be between 1 and 12."),
                new ValidationFailure(nameof(PostPaymentRequest.Amount), "Amount must be greater than zero.")
            });

            _paymentValidator
                .Setup(x => x.ValidateAsync(It.IsAny<PostPaymentRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult); 

            // Act
            var response = await _client.PostAsJsonAsync("/api/Payments", paymentRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorResponse = await response.Content.ReadFromJsonAsync<object>();
            Assert.NotNull(errorResponse);

            _paymentValidator.Verify(x => x.ValidateAsync(It.IsAny<PostPaymentRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            _paymentsService.Verify(x => x.ProcessPaymentAsync(It.IsAny<PostPaymentRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ReturnsInternalServerError_WhenServiceReturnsNull()
        {
            // Arrange
            var paymentRequest = new PostPaymentRequest
            {
                CardNumber = "1234567890123456",
                ExpiryMonth = 12,
                ExpiryYear = 2030,
                Cvv = 123,
                Amount = 100,
                Currency = "GBP"
            };

            var validationResult = new ValidationResult(); 

            _paymentValidator
                .Setup(x => x.ValidateAsync(It.IsAny<PostPaymentRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult); 

            _paymentsService
                .Setup(x => x.ProcessPaymentAsync(It.IsAny<PostPaymentRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PostPaymentResponse?)null); 

            // Act
            var response = await _client.PostAsJsonAsync("/api/Payments", paymentRequest);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            var errorMessage = await response.Content.ReadAsStringAsync();
            Assert.Equal("An unexpected error occurred. Please try again later.", errorMessage);

            _paymentValidator.Verify(x => x.ValidateAsync(It.IsAny<PostPaymentRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            _paymentsService.Verify(x => x.ProcessPaymentAsync(It.IsAny<PostPaymentRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}