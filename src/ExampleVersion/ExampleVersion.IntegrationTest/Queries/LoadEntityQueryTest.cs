using System.Collections.Generic;
using System.Linq;
using affolterNET.Data.Models.Filters;
using affolterNET.Data.Queries;
using ExampleVersion.Data;
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
            CQB<IEnumerable<ExampleVersion_T_DemoTable>>()
                .Arrange(db => new LoadEntityQuery<ExampleVersion_T_DemoTable>())
                .ActAndAssert((result, ah) =>
                {
                    var list = result.Data.ToList();
                    Assert.Single(list);
                    Assert.Equal("It is working!", list.First().Message);
                });
        }

        [Fact]
        public void LoadEntityWithFilterTest()
        {
            var filter = new RootFilter(ExampleVersion_T_DemoTable.Cols.Message)
            {
                Value = "It is working!"
            };
            CQB<IEnumerable<ExampleVersion_T_DemoTable>>()
                .Arrange(db => new LoadEntityQuery<ExampleVersion_T_DemoTable>(filter))
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
            CQB<IEnumerable<ExampleVersion_V_Demo>>()
                .Arrange(db => new LoadEntityQuery<ExampleVersion_V_Demo>())
                .ActAndAssert((result, ah) =>
                {
                    var list = result.Data.ToList();
                    Assert.Single(list);
                    Assert.Equal("It is working!", list.First().Message);
                });
        }
        
        [Fact]
        public void LoadEntityWithFilterInViewTest()
        {
            var filter = new RootFilter(ExampleVersion_V_Demo.Cols.Message)
            {
                Value = "It is working!"
            };
            CQB<IEnumerable<ExampleVersion_V_Demo>>()
                .Arrange(db => new LoadEntityQuery<ExampleVersion_V_Demo>(filter))
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
            CQB<IEnumerable<ExampleVersion_T_DemoTable>>()
                .Arrange(db =>
                {
                    var singleEntry = db.Select<ExampleVersion_T_DemoTable>().ExecuteSingle();
                    return new LoadEntityQuery<ExampleVersion_T_DemoTable>(singleEntry.Id);
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
            CQB<IEnumerable<ExampleVersion_T_DemoTable>>()
                .Arrange(db =>
                {
                    var singleEntry = db.Select<ExampleVersion_T_DemoTable>().ExecuteSingle();
                    var filter = new RootFilter(ExampleVersion_T_DemoTable.Cols.Message)
                    {
                        Value = singleEntry.Message
                    };
                    return new LoadEntityQuery<ExampleVersion_T_DemoTable>(filter);
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