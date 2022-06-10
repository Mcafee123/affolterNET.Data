using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using affolterNET.Data.Extensions;
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
            if (typeof(dbo_T_DemoTable) == typeof(T))
            {
                return new dbo_T_DemoTable();
            }

            if (typeof(dbo_T_DemoTableType) == typeof(T))
            {
                return new dbo_T_DemoTableType();
            }

            if (typeof(dbo_T_History) == typeof(T))
            {
                return new dbo_T_History();
            }

            throw new InvalidOperationException();
        }
    }

    public class ViewFactory : IViewFactory
    {
        public IViewBase Get<T>()
            where T : IViewBase
        {
            if (typeof(dbo_V_Demo) == typeof(T))
            {
                return new dbo_V_Demo();
            }

            throw new InvalidOperationException();
        }
    }

    public class dbo_T_DemoTable : IDtoBase
    {
        public const string TABLE_NAME = "[dbo].[T_DemoTable]";
        [Da.Key]
        public Guid Id { get; set; }

        [Da.MaxLength(1000)]
        public string Message { get; set; }

        [Da.MaxLength(50)]
        public string Status { get; set; }

        public Guid? Type { get; set; }

        private static readonly List<string> colNames = new List<string>{"Id", "Message", "Status", "Type"};
        public IEnumerable<string> GetColumnNames() => colNames;
        public static IEnumerable<string> ColNames => colNames;
        public static class Cols
        {
            public const string Id = "[Id]";
            public const string Message = "[Message]";
            public const string Status = "[Status]";
            public const string Type = "[Type]";
        }

        public bool IsAutoincrementId()
        {
            return false;
        }

        public string GetTableName()
        {
            return TABLE_NAME;
        }

        public string GetSelectCommand(int maxCount = 1000, params string[] excludedColumns)
        {
            var cols = "[Id], [Message], [Type], [Status]".GetColumns(excludedColumns);
            return $"select top({maxCount}) {cols.JoinCols()} from dbo.T_DemoTable where (@Id is null or [Id]=@Id)";
        }

        public string GetInsertCommand(bool returnScopeIdentity = false, params string[] excludedColumns)
        {
            var cols = "[Id], [Message], [Type], [Status]".GetColumns(excludedColumns);
            var sql = $"insert into dbo.T_DemoTable ({cols.JoinCols()}) values ({cols.JoinCols(true)})";
            if (returnScopeIdentity)
            {
                sql += "; select scope_identity() as id;";
            }

            return sql;
        }

        public string GetUpdateCommand(params string[] excludedColumns)
        {
            var cols = "[Id], [Message], [Type], [Status]".GetColumns(excludedColumns);
            return $"update dbo.T_DemoTable set {cols.JoinForUpdate()} where Id=@Id";
        }

        public string GetDeleteCommand()
        {
            return "delete from dbo.T_DemoTable where Id=@Id";
        }

        public string GetDeleteAllCommand()
        {
            return "delete from dbo.T_DemoTable";
        }

        public string GetSaveByIdCommand(bool select = false, params string[] excludedColumns)
        {
            return @$"
                        if exists (select Id from dbo.T_DemoTable where Id = @Id)
                            begin
                                {GetUpdateCommand(excludedColumns)};
                                {"select 'dbo' as [Schema], 'T_DemoTable' as [Table], convert(nvarchar(50), @Id) as [Id], 'updated' as [Action]"}
                            end
                        else
                            begin
                                {GetInsertCommand(false, excludedColumns)}
                                {"select 'dbo' as [Schema], 'T_DemoTable' as [Table], convert(nvarchar(50), @Id) as [Id], 'inserted' as [Action]"}
                            end
                        {(select ? GetSelectCommand(1, excludedColumns) : string.Empty)}";
        }

        public dbo_T_DemoTable GetFromDb(IDbConnection conn, IDbTransaction trsact)
        {
            return conn.QueryFirstOrDefault<dbo_T_DemoTable>(this.GetSelectCommand(1), this, trsact);
        }

        public void Reload(IDbConnection conn, IDbTransaction trsact)
        {
            var loaded = this.GetFromDb(conn, trsact);
            this.Message = loaded.Message;
            this.Status = loaded.Status;
            this.Type = loaded.Type;
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
            return $"Id: {Id}; Message: {Message}; Type: {Type}; Status: {Status}";
        }
    }

    public class dbo_T_DemoTableType : IDtoBase
    {
        public const string TABLE_NAME = "[dbo].[T_DemoTableType]";
        [Da.Key]
        public Guid Id { get; set; }

        [Da.MaxLength(1000)]
        public string Name { get; set; }

        private static readonly List<string> colNames = new List<string>{"Id", "Name"};
        public IEnumerable<string> GetColumnNames() => colNames;
        public static IEnumerable<string> ColNames => colNames;
        public static class Cols
        {
            public const string Id = "[Id]";
            public const string Name = "[Name]";
        }

        public bool IsAutoincrementId()
        {
            return false;
        }

        public string GetTableName()
        {
            return TABLE_NAME;
        }

        public string GetSelectCommand(int maxCount = 1000, params string[] excludedColumns)
        {
            var cols = "[Id], [Name]".GetColumns(excludedColumns);
            return $"select top({maxCount}) {cols.JoinCols()} from dbo.T_DemoTableType where (@Id is null or [Id]=@Id)";
        }

        public string GetInsertCommand(bool returnScopeIdentity = false, params string[] excludedColumns)
        {
            var cols = "[Id], [Name]".GetColumns(excludedColumns);
            var sql = $"insert into dbo.T_DemoTableType ({cols.JoinCols()}) values ({cols.JoinCols(true)})";
            if (returnScopeIdentity)
            {
                sql += "; select scope_identity() as id;";
            }

            return sql;
        }

        public string GetUpdateCommand(params string[] excludedColumns)
        {
            var cols = "[Id], [Name]".GetColumns(excludedColumns);
            return $"update dbo.T_DemoTableType set {cols.JoinForUpdate()} where Id=@Id";
        }

        public string GetDeleteCommand()
        {
            return "delete from dbo.T_DemoTableType where Id=@Id";
        }

        public string GetDeleteAllCommand()
        {
            return "delete from dbo.T_DemoTableType";
        }

        public string GetSaveByIdCommand(bool select = false, params string[] excludedColumns)
        {
            return @$"
                        if exists (select Id from dbo.T_DemoTableType where Id = @Id)
                            begin
                                {GetUpdateCommand(excludedColumns)};
                                {"select 'dbo' as [Schema], 'T_DemoTableType' as [Table], convert(nvarchar(50), @Id) as [Id], 'updated' as [Action]"}
                            end
                        else
                            begin
                                {GetInsertCommand(false, excludedColumns)}
                                {"select 'dbo' as [Schema], 'T_DemoTableType' as [Table], convert(nvarchar(50), @Id) as [Id], 'inserted' as [Action]"}
                            end
                        {(select ? GetSelectCommand(1, excludedColumns) : string.Empty)}";
        }

        public dbo_T_DemoTableType GetFromDb(IDbConnection conn, IDbTransaction trsact)
        {
            return conn.QueryFirstOrDefault<dbo_T_DemoTableType>(this.GetSelectCommand(1), this, trsact);
        }

        public void Reload(IDbConnection conn, IDbTransaction trsact)
        {
            var loaded = this.GetFromDb(conn, trsact);
            this.Name = loaded.Name;
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
            return $"Id: {Id}; Name: {Name}";
        }
    }

    public static class DemoTableTypes
    {
        public static Guid Eins => _dict.First(kvp => kvp.Value == "Eins").Key;
        public static Guid Zwei => _dict.First(kvp => kvp.Value == "Zwei").Key;
        public static Guid Drei => _dict.First(kvp => kvp.Value == "Drei").Key;
        private static Dictionary<Guid, string> _dict = new()
        {{Guid.Parse("c1060bb2-07b0-4e5d-ad0b-35f3993d823d"), "Eins"}, {Guid.Parse("d749abff-6a43-4348-839f-61323fdc52d1"), "Zwei"}, {Guid.Parse("36a072b9-7216-4b99-bf8d-79730a4a1f37"), "Drei"}};
        public static string? GetDemoTableTypesString(this Guid g)
        {
            var entry = _dict.FirstOrDefault(kvp => kvp.Key.Equals(g));
            return entry.Equals(default(KeyValuePair<Guid, string>)) ? null : entry.Value;
        }
    }

    public class dbo_T_History : IDtoBase
    {
        public const string TABLE_NAME = "[dbo].[T_History]";
        [Da.Key]
        public int Id { get; set; }

        public DateTime Applied { get; set; }

        [Da.MaxLength(2000)]
        public string Name { get; set; }

        public string Script { get; set; }

        private static readonly List<string> colNames = new List<string>{"Id", "Applied", "Name", "Script"};
        public IEnumerable<string> GetColumnNames() => colNames;
        public static IEnumerable<string> ColNames => colNames;
        public static class Cols
        {
            public const string Id = "[Id]";
            public const string Applied = "[Applied]";
            public const string Name = "[Name]";
            public const string Script = "[Script]";
        }

        public bool IsAutoincrementId()
        {
            return true;
        }

        public string GetTableName()
        {
            return TABLE_NAME;
        }

        public string GetSelectCommand(int maxCount = 1000, params string[] excludedColumns)
        {
            var cols = "[Id], [Name], [Script], [Applied]".GetColumns(excludedColumns);
            return $"select top({maxCount}) {cols.JoinCols()} from dbo.T_History where (@Id is null or [Id]=@Id)";
        }

        public string GetInsertCommand(bool returnScopeIdentity = false, params string[] excludedColumns)
        {
            var cols = "[Name], [Script], [Applied]".GetColumns(excludedColumns);
            var sql = $"insert into dbo.T_History ({cols.JoinCols()}) values ({cols.JoinCols(true)})";
            if (returnScopeIdentity)
            {
                sql += "; select scope_identity() as id;";
            }

            return sql;
        }

        public string GetUpdateCommand(params string[] excludedColumns)
        {
            var cols = "[Name], [Script], [Applied]".GetColumns(excludedColumns);
            return $"update dbo.T_History set {cols.JoinForUpdate()} where Id=@Id";
        }

        public string GetDeleteCommand()
        {
            return "delete from dbo.T_History where Id=@Id";
        }

        public string GetDeleteAllCommand()
        {
            return "delete from dbo.T_History";
        }

        public string GetSaveByIdCommand(bool select = false, params string[] excludedColumns)
        {
            return @$"
                        if exists (select Id from dbo.T_History where Id = @Id)
                            begin
                                {GetUpdateCommand(excludedColumns)};
                                {"select 'dbo' as [Schema], 'T_History' as [Table], convert(nvarchar(50), @Id) as [Id], 'updated' as [Action]"}
                            end
                        else
                            begin
                                {GetInsertCommand(true, excludedColumns)}
                                {"select 'dbo' as [Schema], 'T_History' as [Table], convert(nvarchar(50), @Id) as [Id], 'inserted' as [Action]"}
                            end
                        {(select ? GetSelectCommand(1, excludedColumns) : string.Empty)}";
        }

        public dbo_T_History GetFromDb(IDbConnection conn, IDbTransaction trsact)
        {
            return conn.QueryFirstOrDefault<dbo_T_History>(this.GetSelectCommand(1), this, trsact);
        }

        public void Reload(IDbConnection conn, IDbTransaction trsact)
        {
            var loaded = this.GetFromDb(conn, trsact);
            this.Applied = loaded.Applied;
            this.Name = loaded.Name;
            this.Script = loaded.Script;
        }

        public string GetIdName()
        {
            return "Id";
        }

        public void SetId(object id)
        {
            var intId = Convert.ToInt32(id);
            Id = intId;
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
            return $"Id: {Id}; Name: {Name}; Script: {Script}; Applied: {Applied}";
        }
    }

    public class dbo_V_Demo : IViewBase
    {
        public const string TABLE_NAME = "[dbo].[V_Demo]";
        public Guid Id { get; set; }

        [Da.MaxLength(1000)]
        public string Message { get; set; }

        [Da.MaxLength(50)]
        public string Status { get; set; }

        public Guid? Type { get; set; }

        private static readonly List<string> colNames = new List<string>{"Id", "Message", "Status", "Type"};
        public IEnumerable<string> GetColumnNames() => colNames;
        public static IEnumerable<string> ColNames => colNames;
        public static class Cols
        {
            public const string Id = "[Id]";
            public const string Message = "[Message]";
            public const string Status = "[Status]";
            public const string Type = "[Type]";
        }

        public string GetTableName()
        {
            return TABLE_NAME;
        }

        public string GetSelectCommand(int maxCount = 1000, params string[] excludedColumns)
        {
            var cols = "[Id], [Message], [Type], [Status]".GetColumns(excludedColumns);
            return $"select top({maxCount}) {cols.JoinCols()} from dbo.V_Demo";
        }

        public string GetInsertCommand(bool returnScopeIdentity = false, params string[] excludedColumns)
        {
            var cols = "[Id], [Message], [Type], [Status]".GetColumns(excludedColumns);
            var sql = $"insert into dbo.V_Demo ({cols.JoinCols()}) values ({cols.JoinCols(true)})";
            if (returnScopeIdentity)
            {
                sql += "; select scope_identity() as id;";
            }

            return sql;
        }

        public string GetUpdateCommand(params string[] excludedColumns)
        {
            throw new InvalidOperationException("no updates on views");
        }

        public string GetDeleteCommand()
        {
            throw new InvalidOperationException("Kein Primary Key");
            ;
        }

        public string GetDeleteAllCommand()
        {
            return "delete from dbo.V_Demo";
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
            return $"Id: {Id}; Message: {Message}; Type: {Type}; Status: {Status}";
        }
    }
}