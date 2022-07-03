using affolterNET.Data.Commands;
using ExampleVersionUserDate.Data;
using Xunit;

namespace ExampleVersionUserDate.IntegrationTest.Commands
{
    [Collection(nameof(ExampleFixture))]
    public class DeleteEntityCommandTest: IntegrationTest
    {
        public DeleteEntityCommandTest(ExampleFixture dbFixture) : base(dbFixture)
        { }
        
        [Fact]
        public void DeleteByIdTest()
        {
            CQB<bool>()
                .Arrange(db =>
                {
                    var singleEntry = db.Select<ExampleVersionUserDate_T_DemoTable>().ExecuteSingle();
                    return new DeleteEntityCommand<ExampleVersionUserDate_T_DemoTable>(singleEntry.Id, singleEntry.VersionTimestamp);
                })
                .ActAndAssert((result, ah) =>
                {
                    Assert.True(result.Data);
                });
        }
    }
}