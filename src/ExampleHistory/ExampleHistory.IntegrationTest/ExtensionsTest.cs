using System;
using ExampleHistory.Data;
using Xunit;

namespace ExampleHistory.IntegrationTest;

public class ExtensionsTest
{
    [Fact]
    public void GetDemoTableTypesStringTestNok()
    {
        var g = Guid.NewGuid().GetDemoTableTypesString();
        Assert.Null(g);
    }
    
    [Fact]
    public void GetDemoTableTypesStringTestOk()
    {
        var g = DemoTableTypes.Eins.GetDemoTableTypesString();
        Assert.Equal("Eins", g);
    }
}