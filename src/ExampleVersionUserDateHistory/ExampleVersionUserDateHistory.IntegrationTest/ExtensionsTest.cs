using System;
using ExampleVersionUserDateHistory.Data;
using Xunit;

namespace ExampleVersionUserDateHistory.IntegrationTest;

public class ExtensionsTest
{
    [Fact]
    public void GetDemoTableTypesStringTestNok()
    {
        var g = Guid.NewGuid().GetExampleVersionUserDateHistoryDemoTableTypesString();
        Assert.Null(g);
    }
    
    [Fact]
    public void GetDemoTableTypesStringTestOk()
    {
        var g = ExampleVersionUserDateHistoryDemoTableTypes.Eins.GetExampleVersionUserDateHistoryDemoTableTypesString();
        Assert.Equal("Eins", g);
    }
}