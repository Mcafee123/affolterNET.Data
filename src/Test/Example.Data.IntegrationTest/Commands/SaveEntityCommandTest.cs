using System;
using affolterNET.Data.Commands;
using affolterNET.Data.Models;
using affolterNET.Data.TestHelpers;
using Example.Data;
using Xunit;

namespace affolterNET.Data.IntegrationTest.Commands
{
    [Collection("ExampleFixture")]
    public class SaveEntityCommandTest: IntegrationTest
    {
        public SaveEntityCommandTest(DbFixture dbFixture) : base(dbFixture)
        {}

        [Fact]
        public void SaveEntityTest()
        {
            CQB<SaveInfo>().Arrange(db =>
            {
                var dto = new dbo_T_DemoTable
                {
                    Id = Guid.NewGuid(),
                    Message = "I was inserted!"
                };
                return new SaveEntityCommand<dbo_T_DemoTable>(dto);
            }).ActAndAssert((result, ah) =>
            {
                
            });
        }
    }
}