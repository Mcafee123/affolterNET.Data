using affolterNET.Data.TestHelpers;
using ExampleUserDate.Data;
using Xunit.Abstractions;

namespace ExampleUserDate.IntegrationTest
{
    public class IntegrationTest : IntegrationTestBase
    {
        public IntegrationTest(ExampleFixture dbFixture, ITestOutputHelper? output = null) : base(dbFixture,
            new DtoFactory(), output)
        {
        }
    }
}