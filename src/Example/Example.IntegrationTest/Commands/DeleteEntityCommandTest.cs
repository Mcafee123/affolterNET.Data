using affolterNET.Data.Commands;
using Example.Data;
using Xunit;

namespace Example.IntegrationTest.Commands
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
                    var singleEntry = db.Select<Example_T_DemoTable>().ExecuteSingle();
                    return new DeleteEntityCommand<Example_T_DemoTable>(singleEntry.Id);
                })
                .ActAndAssert((result, ah) =>
                {
                    Assert.True(result.Data);
                    var sql = result.SqlCommand;
                });
        }
    }
}