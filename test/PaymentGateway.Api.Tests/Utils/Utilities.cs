using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Tests.Utils
{
    public static class Utilities
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
