using System;
using Example.Data;
using Xunit;

namespace Example.IntegrationTest;

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