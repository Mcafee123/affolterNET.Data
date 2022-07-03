using System;
using System.Data.SqlClient;
using affolterNET.Data.Commands;
using affolterNET.Data.Models;
using ExampleVersionUserDate.Data;
using Xunit;

namespace ExampleVersionUserDate.IntegrationTest.Commands
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
                var dto = new ExampleVersionUserDate_T_DemoTable
                {
                    Id = Guid.NewGuid(),
                    Message = "I was inserted!",
                    Type = ExampleVersionUserDateDemoTableTypes.Drei
                };
                return new SaveEntityCommand<ExampleVersionUserDate_T_DemoTable>(dto, "tinu", true, ExampleVersionUserDate_T_DemoTable.Cols.Status);
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
                var dto = new ExampleVersionUserDate_T_DemoTable
                {
                    Id = Guid.NewGuid()
                };
                return new SaveEntityCommand<ExampleVersionUserDate_T_DemoTable>(dto, "tinu", true, ExampleVersionUserDate_T_DemoTable.Cols.Status);
            }).Act());
            Assert.Equal("Cannot insert the value NULL into column 'Message', table 'example.ExampleVersionUserDate.T_DemoTable'; column does not allow nulls. INSERT fails.\nThe statement has been terminated.", ex.Message);
        }
    }
}