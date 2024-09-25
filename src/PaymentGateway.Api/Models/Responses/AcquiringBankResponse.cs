using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Responses;

public class AcquiringBankResponse
{
    [JsonPropertyName("authorized")]
    public bool Authorized { get; set; }

    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; set; } = string.Empty;
}
