using System;
using affolterNET.Data.DtoHelper.Extensions;
using Xunit;

namespace affolterNET.Data.DtoHelper.Test.Extensions;

public class StringExtensionsTest
{
    [Theory]
    [InlineData("T_DemoTable", "TDemoTable")]
    [InlineData("T_DemoTable", "_tDemoTable", true)]
    [InlineData("Is däs ö Dümo (no) / or will - it be???", "_isdaesoeDuemonoorwillitbe", true)]
    [InlineData("demoTable", "DemoTable")]
    public void CleanMemberNameTest(string input, string expectedOutput, bool isField = false)
    {
        var clean = input.CleanMemberName(isField);
        Assert.Equal(expectedOutput, clean);
    }
}