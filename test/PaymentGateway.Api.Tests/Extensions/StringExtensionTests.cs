using AutoFixture;

using PaymentGateway.Api.Extensions;

namespace PaymentGateway.Api.Tests.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("123", 4)]
    [InlineData("123456", 20)]
    public void GetLastChars_Returns_Entire_String_When_Length_Is_Less_Than_Required(string input, int requiredCharLength)
    {
        // Act
        var result = input.GetLastChars(requiredCharLength);

        // Assert
        Assert.Equal(input, result);
    }

    [Theory]
    [InlineData("1234", 4, "1234")]
    [InlineData("1234567", 4, "4567")]
    public void GetLastChars_Returns_Last_Chars_When_Length_Is_Equal_Or_Greater_Than_Required(string input, int requiredCharLength, string expected)
    {
        // Act
        var result = input.GetLastChars(requiredCharLength);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetLastChars_Returns_Empty_String_If_Input_Is_Null()
    {
        // Arrange
        int requiredCharLength = 4;
        string? input = null;
        // Act
        var result = StringExtensions.GetLastChars(input, requiredCharLength);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetLastChars_Returns_Empty_String_If_Input_Is_Empty()
    {
        // Arrange
        int requiredCharLength = 4;

        // Act
        var result = string.Empty.GetLastChars(requiredCharLength);

        // Assert
        Assert.Equal(string.Empty, result);
    }
}

