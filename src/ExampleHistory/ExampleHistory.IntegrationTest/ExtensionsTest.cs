using System;
using ExampleHistory.Data;
using Xunit;

namespace ExampleHistory.IntegrationTest;

public class ExtensionsTest
{
    [Fact]
    public void GetDemoTableTypesStringTestNok()
    {
        var g = Guid.NewGuid().GetExampleHistoryDemoTableTypesString();
        Assert.Null(g);
    }
    
    [Fact]
    public void GetDemoTableTypesStringTestOk()
    {
        var g = ExampleHistoryDemoTableTypes.Eins.GetExampleHistoryDemoTableTypesString();
        Assert.Equal("Eins", g);
    }
}