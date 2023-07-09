using System;
using System.Collections.Generic;
using System.Linq;
using affolterNET.Data.Models.Filters;
using affolterNET.Data.Queries;
using Example.Data;
using Xunit;

namespace Example.IntegrationTest.Queries
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
            CQB<IEnumerable<Example_T_DemoTable>>()
                .Arrange(db => new LoadEntityQuery<Example_T_DemoTable>())
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
            var filter = new RootFilter(Example_T_DemoTable.Cols.Message)
            {
                Value = "It is working!"
            };
            CQB<IEnumerable<Example_T_DemoTable>>()
                .Arrange(db => new LoadEntityQuery<Example_T_DemoTable>(filter))
                .ActAndAssert((result, ah) =>
                {
                    var list = result.Data.ToList();
                    Assert.Single(list);
                    Assert.Equal("It is working!", list.First().Message);
                });
        }

        [Fact]
        public void LoadEntityWithSingleFilterTest()
        {
            var filter = new RootFilter(Example_T_DemoTable.Cols.Message)
            {
                Value = "It is working!"
            };
            CQB<IEnumerable<Example_T_DemoTable>>()
                .Arrange(db =>
                    LoadEntityQuery<Example_T_DemoTable>.CreateWithFilter(Example_T_DemoTable.Cols.Message,
                        "It is working!"))
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
            CQB<IEnumerable<Example_V_Demo>>()
                .Arrange(db => new LoadEntityQuery<Example_V_Demo>())
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
            var filter = new RootFilter(Example_V_Demo.Cols.Message)
            {
                Value = "It is working!"
            };
            CQB<IEnumerable<Example_V_Demo>>()
                .Arrange(db => new LoadEntityQuery<Example_V_Demo>(filter))
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
            CQB<IEnumerable<Example_T_DemoTable>>()
                .Arrange(db =>
                {
                    var singleEntry = db.Select<Example_T_DemoTable>().ExecuteSingle();
                    return new LoadEntityQuery<Example_T_DemoTable>(singleEntry.Id);
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
            CQB<IEnumerable<Example_T_DemoTable>>()
                .Arrange(db =>
                {
                    var singleEntry = db.Select<Example_T_DemoTable>().ExecuteSingle();
                    var filter = new RootFilter(Example_T_DemoTable.Cols.Message)
                    {
                        Value = singleEntry.Message
                    };
                    return new LoadEntityQuery<Example_T_DemoTable>(filter);
                })
                .ActAndAssert((result, ah) =>
                {
                    var list = result.Data.ToList();
                    Assert.Single(list);
                    Assert.Equal("It is working!", list.First().Message);
                });
        }

        [Fact]
        public void LoadWithDateOnlyNullableTest()
        {
            CQB<IEnumerable<Example_T_DemoTable>>()
                .Arrange(db =>
                {
                    db.Insert(new Example_T_DemoTable
                    {
                        Id = Guid.NewGuid(), Message = "hat Ende", DateTest = new DateOnly(2023, 1, 1),
                        DateEndTest = new DateOnly(2024, 1, 1), Status = "geht es?"
                    });
                    return new LoadEntityQuery<Example_T_DemoTable>();
                })
                .ActAndAssert((result, ah) =>
                {
                    Assert.NotNull(result.SqlCommand);
                    Assert.NotEmpty(result.SqlCommand!);
                });
        }
    }
}