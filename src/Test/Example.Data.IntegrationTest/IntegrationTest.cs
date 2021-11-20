using affolterNET.Data.TestHelpers;
using Example.Data;
using Xunit.Abstractions;

namespace affolterNET.Data.IntegrationTest
{
    public class IntegrationTest : IntegrationTestBase
    {
        public IntegrationTest(DbFixture dbFixture, ITestOutputHelper? output = null) : base(dbFixture,
            new DtoFactory(), output)
        {
        }
    }
}