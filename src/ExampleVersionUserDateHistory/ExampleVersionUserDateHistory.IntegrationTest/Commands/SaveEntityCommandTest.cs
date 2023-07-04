using System;
using Microsoft.Data.SqlClient;
using affolterNET.Data.Commands;
using affolterNET.Data.Models;
using ExampleVersionUserDateHistory.Data;
using Xunit;

namespace ExampleVersionUserDateHistory.IntegrationTest.Commands
{
    [Collection(nameof(ExampleFixture))]
    public class SaveEntityCommandTest : IntegrationTest
    {
        public SaveEntityCommandTest(ExampleFixture dbFixture) : base(dbFixture)
        {
        }

        [Fact]
        public void SaveEntityCommandTestOk()
        {
            CQB<SaveInfo>().Arrange(db =>
            {
                var dto = new ExampleVersionUserDateHistory_T_DemoTable
                {
                    Id = Guid.NewGuid(),
                    Message = "I was inserted!",
                    Type = ExampleVersionUserDateHistoryDemoTableTypes.Drei
                };
                return new SaveEntityCommand<ExampleVersionUserDateHistory_T_DemoTable>(dto, "tinu", true, ExampleVersionUserDateHistory_T_DemoTable.Cols.Status);
            }).ActAndAssert((result, ah) =>
            {
                Assert.Equal("inserted", result.Data.Action);
            });
        }

        [Fact]
        public void SaveEntityCommandTestNOk()
        {
            var ex = Assert.Throws<SqlException>(() => CQB<SaveInfo>().Arrange(db =>
            {
                var dto = new ExampleVersionUserDateHistory_T_DemoTable
                {
                    Id = Guid.NewGuid()
                };
                return new SaveEntityCommand<ExampleVersionUserDateHistory_T_DemoTable>(dto, "tinu", true, ExampleVersionUserDateHistory_T_DemoTable.Cols.Status);
            }).Act());
            Assert.Equal("Cannot insert the value NULL into column 'Message', table 'example.ExampleVersionUserDateHistory.T_DemoTable'; column does not allow nulls. INSERT fails.\nThe statement has been terminated.", ex.Message);
        }
    }
}