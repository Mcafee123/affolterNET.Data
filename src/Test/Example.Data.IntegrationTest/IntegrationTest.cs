using System;
using System.Data;
using affolterNET.Data.TestHelpers;
using Dapper;
using Xunit.Abstractions;

namespace Example.Data.IntegrationTest
{
    public class IntegrationTest : IntegrationTestBase
    {
        public IntegrationTest(ExampleFixture dbFixture, ITestOutputHelper? output = null) : base(dbFixture,
            new DtoFactory(), output)
        {
        }
    }
}