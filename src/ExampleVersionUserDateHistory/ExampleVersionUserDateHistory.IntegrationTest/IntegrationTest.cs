using affolterNET.Data.TestHelpers;
using ExampleVersionUserDateHistory.Data;
using Xunit.Abstractions;

namespace ExampleVersionUserDateHistory.IntegrationTest
{
    public class IntegrationTest : IntegrationTestBase
    {
        public IntegrationTest(ExampleFixture dbFixture, ITestOutputHelper? output = null) : base(dbFixture,
            new DtoFactory(), output)
        {
        }
    }
}