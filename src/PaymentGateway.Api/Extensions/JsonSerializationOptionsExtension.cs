using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Extensions
{
    public static class JsonSerializationOptionsExtension
    {
        public static JsonSerializerOptions AddJsonSerializerOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new JsonStringEnumConverter());

            return options;
        }
    }
}
