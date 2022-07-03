using affolterNET.Data.TestHelpers;
using Example.Data;
using Xunit.Abstractions;

namespace Example.IntegrationTest
{
    public class IntegrationTest : IntegrationTestBase
    {
        public IntegrationTest(ExampleFixture dbFixture, ITestOutputHelper? output = null) : base(dbFixture,
            new DtoFactory(), output)
        {
        }
    }
}