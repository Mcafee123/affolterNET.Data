using affolterNET.Data.TestHelpers;
using Xunit;

namespace affolterNET.Data.IntegrationTest.Queries
{
    [Collection("ExampleFixture")]
    public class LoadEntityCommandTest: IntegrationTest
    {
        public LoadEntityCommandTest(DbFixture dbFixture) : base(dbFixture)
        {
            
        }
    }
}