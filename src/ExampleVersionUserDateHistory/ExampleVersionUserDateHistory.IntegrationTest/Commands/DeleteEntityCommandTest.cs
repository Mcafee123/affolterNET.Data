using affolterNET.Data.Commands;
using ExampleVersionUserDateHistory.Data;
using Xunit;

namespace ExampleVersionUserDateHistory.IntegrationTest.Commands
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
                    var singleEntry = db.Select<ExampleVersionUserDateHistory_T_DemoTable>().ExecuteSingle();
                    return new DeleteEntityCommand<ExampleVersionUserDateHistory_T_DemoTable>(singleEntry.Id, singleEntry.VersionTimestamp);
                })
                .ActAndAssert((result, ah) =>
                {
                    Assert.True(result.Data);
                });
        }
    }
}