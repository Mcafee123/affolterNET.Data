using System;
using Example.Data;
using Xunit;

namespace Example.IntegrationTest;

public class ExtensionsTest
{
    [Fact]
    public void GetDemoTableTypesStringTestNok()
    {
        var g = Guid.NewGuid().GetExampleDemoTableTypesString();
        Assert.Null(g);
    }
    
    [Fact]
    public void GetDemoTableTypesStringTestOk()
    {
        var g = ExampleDemoTableTypes.Eins.GetExampleDemoTableTypesString();
        Assert.Equal("Eins", g);
    }
}