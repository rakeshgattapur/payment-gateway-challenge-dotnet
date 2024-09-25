using PaymentGateway.Api.Interfaces.Services;

namespace PaymentGateway.Api.Services
{
    public class CurrencyService : ICurrencyService
    {
        public List<string> GetValidCurrencies()
        {
            return ["USD", "EUR", "GBP"];
        }
    }
}
