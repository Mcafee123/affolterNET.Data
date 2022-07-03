using System;
using ExampleVersionUserDate.Data;
using Xunit;

namespace ExampleVersionUserDate.IntegrationTest;

public class ExtensionsTest
{
    [Fact]
    public void GetDemoTableTypesStringTestNok()
    {
        var g = Guid.NewGuid().GetExampleVersionUserDateDemoTableTypesString();
        Assert.Null(g);
    }
    
    [Fact]
    public void GetDemoTableTypesStringTestOk()
    {
        var g = ExampleVersionUserDateDemoTableTypes.Eins.GetExampleVersionUserDateDemoTableTypesString();
        Assert.Equal("Eins", g);
    }
}