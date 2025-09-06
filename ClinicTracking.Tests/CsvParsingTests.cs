using ClinicTracking.API.Services;
using System.Text;
using Xunit;

namespace ClinicTracking.Tests;

public class CsvParsingTests
{
    [Fact]
    public void ParseCsvLine_WithSimpleFields_ParsesCorrectly()
    {
        // Test the static ParseCsvLine method via reflection since it's private
        var method = typeof(ImportService).GetMethod("ParseCsvLine", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        
        Assert.NotNull(method);
        
        // Test simple CSV line
        var line = "John Smith,MRN001,Chemotherapy,true,false";
        var result = (string[])method.Invoke(null, new object[] { line })!;
        
        Assert.Equal(5, result.Length);
        Assert.Equal("John Smith", result[0]);
        Assert.Equal("MRN001", result[1]);
        Assert.Equal("Chemotherapy", result[2]);
        Assert.Equal("true", result[3]);
        Assert.Equal("false", result[4]);
    }

    [Fact]
    public void ParseCsvLine_WithQuotedFields_ParsesCorrectly()
    {
        var method = typeof(ImportService).GetMethod("ParseCsvLine", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        
        Assert.NotNull(method);
        
        // Test CSV line with quoted fields containing commas and quotes
        var line = "\"Smith, John\",MRN001,\"Treatment with, commas\",\"Comments with \"\"quotes\"\"\",true";
        var result = (string[])method.Invoke(null, new object[] { line })!;
        
        Assert.Equal(5, result.Length);
        Assert.Equal("Smith, John", result[0]);
        Assert.Equal("MRN001", result[1]);
        Assert.Equal("Treatment with, commas", result[2]);
        Assert.Equal("Comments with \"quotes\"", result[3]);
        Assert.Equal("true", result[4]);
    }

    [Fact]
    public void ParseCsvLine_WithEmptyFields_ParsesCorrectly()
    {
        var method = typeof(ImportService).GetMethod("ParseCsvLine", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        
        Assert.NotNull(method);
        
        // Test CSV line with empty fields
        var line = "John Smith,,Treatment,,true";
        var result = (string[])method.Invoke(null, new object[] { line })!;
        
        Assert.Equal(5, result.Length);
        Assert.Equal("John Smith", result[0]);
        Assert.Equal("", result[1]);
        Assert.Equal("Treatment", result[2]);
        Assert.Equal("", result[3]);
        Assert.Equal("true", result[4]);
    }
}