using System;
using System.Data.SqlClient;
using affolterNET.Data.Commands;
using affolterNET.Data.Models;
using ExampleVersion.Data;
using Xunit;

namespace ExampleVersion.IntegrationTest.Commands
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
                var dto = new ExampleVersion_T_DemoTable
                {
                    Id = Guid.NewGuid(),
                    Message = "I was inserted!",
                    Type = ExampleVersionDemoTableTypes.Drei
                };
                return new SaveEntityCommand<ExampleVersion_T_DemoTable>(dto, "tinu", true, ExampleVersion_T_DemoTable.Cols.Status);
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
                var dto = new ExampleVersion_T_DemoTable
                {
                    Id = Guid.NewGuid()
                };
                return new SaveEntityCommand<ExampleVersion_T_DemoTable>(dto, "tinu", true, ExampleVersion_T_DemoTable.Cols.Status);
            }).Act());
            Assert.Equal("Cannot insert the value NULL into column 'Message', table 'example.ExampleVersion.T_DemoTable'; column does not allow nulls. INSERT fails.\nThe statement has been terminated.", ex.Message);
        }
    }
}