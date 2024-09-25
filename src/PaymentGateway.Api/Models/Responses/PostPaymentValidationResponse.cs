using PaymentGateway.Api.Enums;

namespace PaymentGateway.Api.Models.Responses
{
    public class PostPaymentValidationResponse
    {
        public PaymentStatus Status => PaymentStatus.Rejected;
        public List<string> Errors { get; set; } = new();
    }
}
