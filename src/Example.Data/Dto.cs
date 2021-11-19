using System;
using System.Collections.Generic;
using System.Data;
using affolterNET.Data.Interfaces;
using Dapper;
using Da = System.ComponentModel.DataAnnotations;

#pragma warning disable CS8618
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable StyleCop.SA1001
// ReSharper disable StyleCop.SA1402
// ReSharper disable StyleCop.SA1101
// ReSharper disable StyleCop.SA1310
// ReSharper disable StyleCop.SA1201
// ReSharper disable StyleCop.SA1401
// ReSharper disable StyleCop.SA1311
// ReSharper disable StyleCop.SA1516
// ReSharper disable StyleCop.SA1015
// ReSharper disable StyleCop.SA1012
// ReSharper disable StyleCop.SA1013
// ReSharper disable StyleCop.SA1113
// ReSharper disable StyleCop.SA1115
// ReSharper disable StyleCop.SA1116
namespace Example.Data
{
    public class DtoFactory : IDtoFactory
    {
        public IDtoBase Get<T>()
            where T : IDtoBase
        {
            if (typeof(dbo_DemoTable) == typeof(T))
            {
                return new dbo_DemoTable();
            }

            throw new InvalidOperationException();
        }
    }

    public class ViewFactory : IViewFactory
    {
        public IViewBase Get<T>()
            where T : IViewBase
        {
            throw new InvalidOperationException();
        }
    }

    public class dbo_DemoTable : IDto
    {
        public const string TABLE_NAME = "[dbo].[DemoTable]";
        [Da.Key]
        public Guid Id { get; set; }

        [Da.MaxLength(1000)]
        public string Message { get; set; }

        private static readonly List<string> colNames = new List<string>{"Id", "Message"}; public  static  IEnumerable < string > ColNames  =>  colNames ;  public  static  class  Cols { public  const  string  Id  =  "[Id]" ;  public  const  string  Message  =  "[Message]" ;  }
        public bool IsAutoincrementId()
        {
            return false;
        }

        public string GetTableName()
        {
            return TABLE_NAME;
        }

        public string GetSelectCommand(int maxCount = 1000)
        {
            return $"select top({maxCount}) [Id], [Message] from dbo.DemoTable where (@Id is null or [Id]=@Id)";
        }

        public string GetInsertCommand(bool returnScopeIdentity = false)
        {
            var sql = "insert into dbo.DemoTable (Id, Message) values (@Id, @Message)";
            if (returnScopeIdentity)
            {
                sql += "; select scope_identity() as id;";
            }

            return sql;
        }

        public string GetUpdateCommand()
        {
            return "update dbo.DemoTable set Message=@Message where Id=@Id";
        }

        public string GetDeleteCommand()
        {
            return "delete from dbo.DemoTable where Id=@Id";
        }

        public string GetDeleteAllCommand()
        {
            return "delete from dbo.DemoTable";
        }

        public string GetSaveByIdCommand(bool select = false)
        {
            return @$"
                        if exists (select Id from dbo.DemoTable where Id = @Id)
                            begin
                                {GetUpdateCommand()};
                                {(select ? string.Empty : "select 'dbo' as [Schema], 'DemoTable' as [Table], convert(nvarchar(50), @Id) as [Id], 'updated' as [Action]")}
                            end
                        else
                            begin
                                {GetInsertCommand(false)}
                                {(select ? string.Empty : "select 'dbo' as [Schema], 'DemoTable' as [Table], convert(nvarchar(50), @Id) as [Id], 'inserted' as [Action]")}
                            end
                        {(select ? GetSelectCommand() : string.Empty)}";
        }

        public dbo_DemoTable GetFromDb(IDbConnection conn, IDbTransaction trsact)
        {
            return conn.QueryFirstOrDefault<dbo_DemoTable>(this.GetSelectCommand(1), this, trsact);
        }

        public void Reload(IDbConnection conn, IDbTransaction trsact)
        {
            var loaded = this.GetFromDb(conn, trsact);
            this.Message = loaded.Message;
        }

        public string GetIdName()
        {
            return "Id";
        }

        public void SetId(object id)
        {
            if (!Guid.TryParse(id.ToString(), out var guidId))
            {
                throw new InvalidOperationException("invalid id");
            }

            Id = guidId;
        }

        public string GetVersionName()
        {
            return "n.a.";
        }

        public string GetIsActiveName()
        {
            return "n.a.";
        }

        public void SetIsActive(bool isActive)
        {
        }

        public string GetUpdatedUserName()
        {
            return "n.a.";
        }

        public void SetUpdatedUser(string userName)
        {
        }

        public string GetInsertedUserName()
        {
            return "n.a.";
        }

        public void SetInsertedUser(string userName)
        {
        }

        public string GetUpdatedDateName()
        {
            return "n.a.";
        }

        public void SetUpdatedDate(DateTime date)
        {
        }

        public string GetInsertedDateName()
        {
            return "n.a.";
        }

        public void SetInsertedDate(DateTime date)
        {
        }

        public override string ToString()
        {
            return $"Id: {Id}; Message: {Message}";
        }
    }
}