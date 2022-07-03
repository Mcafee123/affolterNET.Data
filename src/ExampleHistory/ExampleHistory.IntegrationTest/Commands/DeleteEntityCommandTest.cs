using affolterNET.Data.Commands;
using Xunit;

namespace ExampleHistory.IntegrationTest.Commands
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
                    var singleEntry = db.Select<dbo_T_DemoTable>().ExecuteSingle();
                    return new DeleteEntityCommand<dbo_T_DemoTable>(singleEntry.Id, singleEntry.VersionTimestamp);
                })
                .ActAndAssert((result, ah) =>
                {
                    Assert.True(result.Data);
                });
        }
    }
}