using ClinicTracking.Client.Components.Common;

namespace ClinicTracking.Tests;

public class InputDateNullableTests
{
    [Fact]
    public void InputDateNullable_TryParseValueFromString_WithEmptyString_ShouldReturnNull()
    {
        // Arrange
        var component = new InputDateNullable();
        
        // Act
        var result = component.GetType()
            .GetMethod("TryParseValueFromString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(component, new object?[] { "", null!, null! });

        // Assert - The method should return true (indicating successful parsing)
        Assert.NotNull(result);
        Assert.True((bool)result);
    }

    [Fact]
    public void InputDateNullable_TryParseValueFromString_WithNullString_ShouldReturnNull()
    {
        // Arrange
        var component = new InputDateNullable();
        
        // Act
        var result = component.GetType()
            .GetMethod("TryParseValueFromString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(component, new object?[] { null, null!, null! });

        // Assert - The method should return true (indicating successful parsing)
        Assert.NotNull(result);
        Assert.True((bool)result);
    }

    [Fact]
    public void InputDateNullable_TryParseValueFromString_WithWhitespace_ShouldReturnNull()
    {
        // Arrange
        var component = new InputDateNullable();
        
        // Act
        var result = component.GetType()
            .GetMethod("TryParseValueFromString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(component, new object?[] { "   ", null!, null! });

        // Assert - The method should return true (indicating successful parsing)
        Assert.NotNull(result);
        Assert.True((bool)result);
    }

    [Fact]
    public void InputDateNullable_FormatValueAsString_WithNullValue_ShouldReturnNull()
    {
        // Arrange
        var component = new InputDateNullable();
        DateTime? nullDate = null;
        
        // Act
        var result = component.GetType()
            .GetMethod("FormatValueAsString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(component, new object?[] { nullDate });

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void InputDateNullable_FormatValueAsString_WithValidDate_ShouldReturnFormattedString()
    {
        // Arrange
        var component = new InputDateNullable();
        DateTime? testDate = new DateTime(2025, 9, 5);
        
        // Act
        var result = component.GetType()
            .GetMethod("FormatValueAsString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(component, new object?[] { testDate });

        // Assert
        Assert.NotNull(result);
        Assert.Equal("2025-09-05", result);
    }
}