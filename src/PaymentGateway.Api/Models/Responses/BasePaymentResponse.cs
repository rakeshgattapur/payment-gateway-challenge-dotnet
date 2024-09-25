using PaymentGateway.Api.Enums;

namespace PaymentGateway.Api.Models.Responses
{
    public class BasePaymentResponse
    {
        public Guid Id { get; set; }
        public PaymentStatus Status { get; set; }
        public string CardNumberLastFourDigits { get; set; } = string.Empty;
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string Currency { get; set; } = string.Empty;
        public int Amount { get; set; }
    }
}
