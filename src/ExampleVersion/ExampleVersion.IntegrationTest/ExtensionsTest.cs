using System;
using ExampleVersion.Data;
using Xunit;

namespace ExampleVersion.IntegrationTest;

public class ExtensionsTest
{
    [Fact]
    public void GetDemoTableTypesStringTestNok()
    {
        var g = Guid.NewGuid().GetExampleVersionDemoTableTypesString();
        Assert.Null(g);
    }
    
    [Fact]
    public void GetDemoTableTypesStringTestOk()
    {
        var g = ExampleVersionDemoTableTypes.Eins.GetExampleVersionDemoTableTypesString();
        Assert.Equal("Eins", g);
    }
}