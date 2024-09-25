using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Requests
{
    public class AcquiringBankRequest
    {
        public AcquiringBankRequest(string cardNumber, string expiryDate, string currency, int amount, string cvv)
        {
            CardNumber = cardNumber;
            ExpiryDate = expiryDate;
            Currency = currency;
            Amount = amount;
            Cvv = cvv;
        }

        [JsonPropertyName("card_number")]
        public string CardNumber { get; set; } = string.Empty;

        [JsonPropertyName("expiry_date")]
        public string ExpiryDate { get; set; } = string.Empty;

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("cvv")]
        public string Cvv { get; set; } = string.Empty;
    }
}