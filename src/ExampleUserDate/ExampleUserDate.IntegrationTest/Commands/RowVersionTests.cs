using System;
using affolterNET.Data;
using affolterNET.Data.Commands;
using affolterNET.Data.Models;
using Xunit;
using Xunit.Abstractions;

namespace ExampleHistory.IntegrationTest.Commands;

[Collection(nameof(ExampleFixture))]
public class RowVersionTests : IntegrationTest
{
    public RowVersionTests(ExampleFixture dbFixture, ITestOutputHelper log) : base(dbFixture)
    {

    }

    [Fact]
    public void CanInsert()
    {
        CQB<SaveInfo>()
            .Arrange(db =>
            {
                var dto = new dbo_T_DemoTable
                {
                    Id = Guid.NewGuid(),
                    Message = "Ich bin drin",
                    Status = "Neu",
                };
                dto.SetInserted("test");
                return new SaveEntityCommand<dbo_T_DemoTable>(dto);
            })
            .ActAndAssert((result, ah) =>
            {
                Assert.Equal(Constants.Inserted, result.Data.Action);
            });
    }
    
    [Fact]
    public void CanUpdate()
    {
        CQB<SaveInfo>()
            .Arrange(db =>
            {
                var dto = new dbo_T_DemoTable
                {
                    Id = Guid.NewGuid(),
                    Message = "Ich bin drin",
                    Status = "Neu",
                };
                dto.SetInserted("test");
                db.Insert(dto);
                var reloaded = db.SelectById<dbo_T_DemoTable>(dto.Id);
                Assert.NotNull(reloaded);
                reloaded.Message = "I was reloaded";
                return new SaveEntityCommand<dbo_T_DemoTable>(reloaded);
            })
            .ActAndAssert((result, ah) =>
            {
                Assert.Equal(Constants.Updated, result.Data.Action);
            });
    }
    
    [Fact]
    public void CanNotUpdateWhenChanged()
    {
        CQB<SaveInfo>()
            .Arrange(db =>
            {
                var dto = new dbo_T_DemoTable
                {
                    Id = Guid.NewGuid(),
                    Message = "Ich bin drin",
                    Status = "Neu",
                };
                dto.SetInserted("test");
                db.Insert(dto);
                var reloaded = db.SelectById<dbo_T_DemoTable>(dto.Id);
                Assert.NotNull(reloaded);
                
                // different update
                db.Update<dbo_T_DemoTable>().WithUpdate(dbo_T_DemoTable.Cols.Message.StripSquareBrackets(), "somebody else").Execute();
                
                reloaded.Message = "I was reloaded";
                return new SaveEntityCommand<dbo_T_DemoTable>(reloaded);
            })
            .ActAndAssert((result, ah) =>
            {
                Assert.Equal(Constants.NoAction, result.Data.Action);
            });
    }
    
    [Fact]
    public void CanNotUpdateWithoutTimestamp()
    {
        CQB<SaveInfo>()
            .Arrange(db =>
            {
                var dto = new dbo_T_DemoTable
                {
                    Id = Guid.NewGuid(),
                    Message = "Ich bin drin",
                    Status = "Neu",
                };
                dto.SetInserted("test");
                db.Insert(dto);
                var reloaded = db.SelectById<dbo_T_DemoTable>(dto.Id);
                Assert.NotNull(reloaded);
                reloaded.VersionTimestamp = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 };
                reloaded.Message = "I was reloaded";
                return new SaveEntityCommand<dbo_T_DemoTable>(reloaded);
            })
            .ActAndAssert((result, ah) =>
            {
                Assert.Equal(Constants.NoAction, result.Data.Action);
            });
    }
}