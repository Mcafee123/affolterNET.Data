using affolterNET.Data.TestHelpers;
using ExampleVersion.Data;
using Xunit.Abstractions;

namespace ExampleVersion.IntegrationTest
{
    public class IntegrationTest : IntegrationTestBase
    {
        public IntegrationTest(ExampleFixture dbFixture, ITestOutputHelper? output = null) : base(dbFixture,
            new DtoFactory(), output)
        {
        }
    }
}