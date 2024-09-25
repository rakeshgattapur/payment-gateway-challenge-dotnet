namespace PaymentGateway.Api.Interfaces.Services
{
    public interface ICurrencyService
    {
        List<string> GetValidCurrencies();
    }
}