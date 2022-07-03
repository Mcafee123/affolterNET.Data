using System.Collections.Generic;
using System.Linq;
using affolterNET.Data.Models.Filters;
using affolterNET.Data.Queries;
using Xunit;

namespace ExampleVersion.IntegrationTest.Queries
{
    [Collection(nameof(ExampleFixture))]
    public class LoadEntityQueryTest : IntegrationTest
    {
        public LoadEntityQueryTest(ExampleFixture dbFixture) : base(dbFixture)
        {
        }

        [Fact]
        public void LoadAllEntitiesTest()
        {
            CQB<IEnumerable<dbo_T_DemoTable>>()
                .Arrange(db => new LoadEntityQuery<dbo_T_DemoTable>())
                .ActAndAssert((result, ah) =>
                {
                    var list = result.Data.ToList();
                    Assert.Single(list);
                    Assert.Equal("It is working!", list.First().Message);
                });
        }

        [Fact]
        public void LoadAllEntitiesInViewTest()
        {
            CQB<IEnumerable<dbo_V_Demo>>()
                .Arrange(db => new LoadEntityQuery<dbo_V_Demo>())
                .ActAndAssert((result, ah) =>
                {
                    var list = result.Data.ToList();
                    Assert.Single(list);
                    Assert.Equal("It is working!", list.First().Message);
                });
        }
        
        [Fact]
        public void LoadByIdTest()
        {
            CQB<IEnumerable<dbo_T_DemoTable>>()
                .Arrange(db =>
                {
                    var singleEntry = db.Select<dbo_T_DemoTable>().ExecuteSingle();
                    return new LoadEntityQuery<dbo_T_DemoTable>(singleEntry.Id);
                })
                .ActAndAssert((result, ah) =>
                {
                    var list = result.Data.ToList();
                    Assert.Single(list);
                    Assert.Equal("It is working!", list.First().Message);
                });
        }

        [Fact]
        public void LoadByOtherTest()
        {
            CQB<IEnumerable<dbo_T_DemoTable>>()
                .Arrange(db =>
                {
                    var singleEntry = db.Select<dbo_T_DemoTable>().ExecuteSingle();
                    var filter = new RootFilter(dbo_T_DemoTable.Cols.Message)
                    {
                        Value = singleEntry.Message
                    };
                    return new LoadEntityQuery<dbo_T_DemoTable>(filter);
                })
                .ActAndAssert((result, ah) =>
                {
                    var list = result.Data.ToList();
                    Assert.Single(list);
                    Assert.Equal("It is working!", list.First().Message);
                });
        }
    }
}