using affolterNET.Data.TestHelpers;
using ExampleVersionUserDate.Data;
using Xunit.Abstractions;

namespace ExampleVersionUserDate.IntegrationTest
{
    public class IntegrationTest : IntegrationTestBase
    {
        public IntegrationTest(ExampleFixture dbFixture, ITestOutputHelper? output = null) : base(dbFixture,
            new DtoFactory(), output)
        {
        }
    }
}