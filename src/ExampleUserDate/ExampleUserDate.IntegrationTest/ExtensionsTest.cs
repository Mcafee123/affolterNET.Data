using System;
using ExampleUserDate.Data;
using Xunit;

namespace ExampleUserDate.IntegrationTest;

public class ExtensionsTest
{
    [Fact]
    public void GetDemoTableTypesStringTestNok()
    {
        var g = Guid.NewGuid().GetExampleUserDateDemoTableTypesString();
        Assert.Null(g);
    }
    
    [Fact]
    public void GetDemoTableTypesStringTestOk()
    {
        var g = ExampleUserDateDemoTableTypes.Eins.GetExampleUserDateDemoTableTypesString();
        Assert.Equal("Eins", g);
    }
}