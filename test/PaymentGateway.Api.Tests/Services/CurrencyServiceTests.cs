using PaymentGateway.Api.Interfaces.Services;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.Services
{
    public class CurrencyServiceTests
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyServiceTests()
        {
            _currencyService = new CurrencyService();
        }

        [Fact]
        public void GetValidCurrencies_Should_Return_Valid_Currencies()
        {
            // Act
            var result = _currencyService.GetValidCurrencies();

            // Assert
            var expectedCurrencies = new List<string> { "USD", "EUR", "GBP" };
            Assert.Equal(expectedCurrencies, result);
        }
    }
}
