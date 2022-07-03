using affolterNET.Data.TestHelpers;
using ExampleHistory.Data;
using Xunit.Abstractions;

namespace ExampleHistory.IntegrationTest
{
    public class IntegrationTest : IntegrationTestBase
    {
        public IntegrationTest(ExampleFixture dbFixture, ITestOutputHelper? output = null) : base(dbFixture,
            new DtoFactory(), output)
        {
        }
    }
}