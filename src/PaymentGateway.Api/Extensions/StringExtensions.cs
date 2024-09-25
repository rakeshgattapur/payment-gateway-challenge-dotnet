namespace PaymentGateway.Api.Extensions
{
    public static class StringExtensions
    {
        public static string GetLastChars(this string input, int requiredCharLength)
        {
            if (string.IsNullOrEmpty(input) || input.Length < requiredCharLength)
                return input;

            return input[^requiredCharLength..];
        }
    }
}
