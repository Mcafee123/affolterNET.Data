using System;
using System.Collections.Generic;
using System.Linq;
using affolterNET.Data.Queries;
using Xunit;

namespace Example.Data.IntegrationTest.Queries
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
                    return new LoadEntityQuery<dbo_T_DemoTable>(1000,
                        new Tuple<string, object>(dbo_T_DemoTable.Cols.Message, singleEntry.Message));
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