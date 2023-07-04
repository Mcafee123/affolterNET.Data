using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using affolterNET.Data.DtoHelper.CodeGen;

namespace affolterNET.Data.DtoHelper.Database
{
    [SuppressMessage(
        "StyleCop.CSharp.OrderingRules",
        "SA1201:ElementsMustAppearInTheCorrectOrder",
        Justification = "Reviewed. Suppression is OK here.")]
    public class Table : ElementBase
    {
        private const string ColumnSql = @"SELECT 
			TABLE_CATALOG AS [Database],
			TABLE_SCHEMA AS Owner, 
			TABLE_NAME AS TableName, 
			COLUMN_NAME AS ColumnName, 
			ORDINAL_POSITION AS OrdinalPosition, 
			COLUMN_DEFAULT AS DefaultSetting, 
			IS_NULLABLE AS IsNullable, DATA_TYPE AS DataType, 
			CHARACTER_MAXIMUM_LENGTH AS MaxLength, 
			DATETIME_PRECISION AS DatePrecision,
			COLUMNPROPERTY(object_id('[' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']'), COLUMN_NAME, 'IsIdentity') AS IsIdentity,
			COLUMNPROPERTY(object_id('[' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']'), COLUMN_NAME, 'IsComputed') as IsComputed
		FROM  INFORMATION_SCHEMA.COLUMNS
		WHERE TABLE_NAME=@tableName AND TABLE_SCHEMA=@schemaName
		ORDER BY OrdinalPosition ASC";

        private const string GetPkSql = @"SELECT COLUMN_NAME as ColumnName
            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
            JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu ON tc.CONSTRAINT_NAME = ccu.Constraint_name
            WHERE tc.TABLE_NAME = @tableName and tc.CONSTRAINT_TYPE = 'Primary Key'";

        private static readonly Regex RxCleanUp = new Regex(@"[^\w\d_]", RegexOptions.Compiled);

        private static readonly Func<string, string> CleanUp = str =>
        {
            str = RxCleanUp.Replace(str, "_");
            if (char.IsDigit(str[0]))
            {
                str = "_" + str;
            }

            return str;
        };

        internal readonly List<Column> AllColumns;

        private readonly GeneratorCfg cfg;

        private string cleanName = null!;

        public Table(GeneratorCfg cfg)
        {
            InnerKeys = new List<Key>();
            OuterKeys = new List<Key>();
            AllColumns = new List<Column>();
            this.cfg = cfg;
        }

        public bool InsertedUpdatedDateUtc => cfg.InsertedUpdatedDateUtc;

        public string? ClassName { get; set; }

        public string CleanName
        {
            get => cleanName;
            set => cleanName = CleanUp(value);
        }

        public IEnumerable<Column> Columns =>
            AllColumns.Where(c => !c.Ignore).OrderByDescending(c => c.IsPK).ThenBy(c => c.IsDefaultCol())
                .ThenBy(c => c.PropertyName);

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string? DebugText { get; set; }

        public string FullName { get; set; } = null!;

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public bool Ignore { get; set; }

        // ReSharper disable once CollectionNeverQueried.Global
        public List<Key> InnerKeys { get; }

        public bool IsView { get; set; }

        // ReSharper disable once StyleCop.SA1126
        public Column this[string columnName] => GetColumn(columnName);

        public string Name { get; set; } = null!;

        public string? ObjectName { get; set; }

        // ReSharper disable once CollectionNeverQueried.Global
        public List<Key> OuterKeys { get; }

        public string Schema { get; set; } = null!;

        public Column GetPrimaryKeyColumn()
        {
            var pkCol = AllColumns.FirstOrDefault(x => x.IsPK);
            if (pkCol == null)
            {
                throw new InvalidOperationException($"Tabelle {Schema}.{Name} hat keine Primary Key Column");
            }

            return pkCol;
        }
        
        public IEnumerable<Column> GetPrimaryKeyColumns()
        {
            return AllColumns.Where(x => x.IsPK);
        }

        public Column GetColumn(string columnName)
        {
            return AllColumns.Single(x => string.Compare(x.Name, columnName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public override string ToString()
        {
            return string.Join("; ", AllColumns.Select(c => string.Format("{0}: {{{0}}}", c.Name)));
        }

        public void LoadColumns(SqlConnection connection)
        {
            AllColumns.Clear();
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = ColumnSql;

                var p = cmd.CreateParameter();
                p.ParameterName = "@tableName";
                p.Value = Name;
                cmd.Parameters.Add(p);

                p = cmd.CreateParameter();
                p.ParameterName = "@schemaName";
                p.Value = Schema;
                cmd.Parameters.Add(p);

                using (IDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        if (cfg.IsColumnExcluded(rdr["ColumnName"].ToString()!))
                        {
                            continue;
                        }

                        var col = new Column(cfg)
                        {
                            Name = rdr["ColumnName"].ToString()!,
                            MaxLength = rdr["MaxLength"] as int?
                        };

                        col.PropertyName = CleanUp(col.Name);
                        var dataType = rdr["DataType"].ToString()!;
                        col.DataType = dataType;
                        col.PropertyType = GetPropertyType(dataType);
                        col.IsNullable = rdr["IsNullable"].ToString() == "YES";
                        col.IsAutoIncrement = (int)rdr["IsIdentity"] == 1;
                        if (col.MaxLength < 1)
                        {
                            col.MaxLength = null;
                        }

                        AllColumns.Add(col);
                    }
                }
            }
        }

        private string GetPropertyType(string sqlType)
        {
            var sysType = "string";
            switch (sqlType)
            {
                case "bigint":
                    sysType = "long";
                    break;
                case "smallint":
                    sysType = "short";
                    break;
                case "int":
                    sysType = "int";
                    break;
                case "uniqueidentifier":
                    sysType = "Guid";
                    break;
                case "date":
                    sysType = nameof(DateOnly);
                    break;
                case "smalldatetime":
                case "datetime":
                case "datetime2":
                case "time":
                    sysType = "DateTime";
                    break;
                case "datetimeoffset":
                    sysType = nameof(DateTimeOffset);
                    break;
                case "float":
                    sysType = "double";
                    break;
                case "real":
                    sysType = "float";
                    break;
                case "numeric":
                case "smallmoney":
                case "decimal":
                case "money":
                    sysType = "decimal";
                    break;
                case "tinyint":
                    sysType = "byte";
                    break;
                case "bit":
                    sysType = "bool";
                    break;
                case "image":
                case "binary":
                case "varbinary":
                case "timestamp":
                    sysType = "byte[]";
                    break;
                case "geography":
                    sysType = "Microsoft.SqlServer.Types.SqlGeography";
                    break;
                case "geometry":
                    sysType = "Microsoft.SqlServer.Types.SqlGeometry";
                    break;
            }

            return sysType;
        }

        public void MarkPrimaryKeys(SqlConnection connection)
        {
            var primaryKeys = GetPk(connection);

            foreach (var primaryKey in primaryKeys)
            {
                var pkColumn = AllColumns.SingleOrDefault(x => x.Name?.ToLower().Trim() == primaryKey.ToLower().Trim());
                if (pkColumn != null)
                {
                    pkColumn.IsPK = true;
                }
            }
        }

        private string[] GetPk(SqlConnection connection)
        {
            var primaryKeys = new List<string>();

            using (var cmd = new SqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = GetPkSql;

                var p = cmd.CreateParameter();
                p.ParameterName = "@tableName";
                p.Value = Name;
                cmd.Parameters.Add(p);

                using (var result = cmd.ExecuteReader())
                {
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            primaryKeys.Add(result.GetString(0));
                        }
                    }
                }
            }

            return primaryKeys.ToArray();
        }

        public string WriteDefaults(bool deleteonly = false)
        {
            var delete = $@"
declare @constraint_name nvarchar(300)
declare constraints_cursor CURSOR FOR 
select 
cs.[name] as ConstraintName
-- ,cl.[name]
-- ,cs.* 
from sys.default_constraints cs 
join sys.columns cl on cs.parent_column_id = cl.column_id and cs.parent_object_id = cl.[object_id]
where 
parent_object_id = OBJECT_ID('{Schema}.{Name}')
and (cl.[name] = 'ErstelltDurch' or cl.[name] = 'ErstelltAm' OR cl.[name] = 'IstAktiv')

open constraints_cursor

fetch next from constraints_cursor   
into @constraint_name

while @@FETCH_STATUS = 0  
begin
	exec('alter table {Schema}.{Name} drop constraint ' + @constraint_name)

	fetch next from constraints_cursor   
	into @constraint_name
end
close constraints_cursor 
deallocate constraints_cursor
";
            if (deleteonly)
            {
                return delete;
            }

            var constraints = $@"
alter table {Schema}.{Name} add constraint DF_{Schema}_{Name}_IstAktiv DEFAULT 1 for IstAktiv
alter table {Schema}.{Name} add constraint DF_{Schema}_{Name}_ErstelltAm DEFAULT getdate() for ErstelltAm
";
            return delete + constraints;
        }

        public string WriteUpdateTrigger(string updateDateName, string updateUserName, bool deleteonly = false)
        {
            if (Columns.All(c => c.Name != updateDateName) || Columns.All(c => c.Name != updateUserName))
            {
                return $"-- {Schema}.{Name}: Update-Trigger wird aufgrund fehlender Spalten nicht erstellt";
            }

            var trg = $@"
create trigger {Schema}.{Name}_Update
on {Schema}.{Name}
for update
as
begin
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    set nocount on

	IF UPDATE (ErstelltAm) 
		throw 60002, 'Es sind keine Updates des Attributs ErstelltAm zugelassen', 1

	IF UPDATE (ErstelltDurch) 
		throw 60003, 'Es sind keine Updates des Attributs ErstelltDurch zugelassen', 1

	IF NOT UPDATE ({updateDateName}) 
		throw 60004, '{updateDateName} muss gesetzt werden', 1

	IF NOT UPDATE ({updateUserName}) 
		throw 60005, '{updateUserName} muss gesetzt werden', 1
end";
            return trg;
        }

        public string WriteDeleteAllTriggers()
        {
            var tableName = $"{Schema}.{Name}";
            var delete = $@"
declare @trgName varchar(200)
while exists (select * from sys.triggers where parent_id = OBJECT_ID(N'{tableName}'))
	begin
		set @trgName = (select top(1) name from sys.triggers where parent_id = OBJECT_ID(N'{tableName}'))
		exec('drop trigger {Schema}.' + @trgName)
	end";
            return delete;
        }

        public string GetVersionName()
        {
            var versionCol = Columns.FirstOrDefault(c => cfg.VersionFunc(c.Name));
            if (versionCol != null && !string.IsNullOrWhiteSpace(versionCol.Name))
            {
                return versionCol.Name;
            }

            return Constants.NotAvailable;
        }

        public string GetIsActiveName()
        {
            var isActiveCol = Columns.FirstOrDefault(c => cfg.IsActiveFunc(c.Name));
            if (isActiveCol != null && !string.IsNullOrWhiteSpace(isActiveCol.Name))
            {
                return isActiveCol.Name;
            }

            return Constants.NotAvailable;
        }

        public string GetUpdatedUserName()
        {
            var updateUserCol = Columns.FirstOrDefault(c => cfg.UpdateUserFunc(c.Name));
            if (updateUserCol != null && !string.IsNullOrWhiteSpace(updateUserCol.Name))
            {
                return updateUserCol.Name;
            }

            return Constants.NotAvailable;
        }

        public string GetInsertedUserName()
        {
            var insertUserCol = Columns.FirstOrDefault(c => cfg.InsertUserFunc(c.Name));
            if (insertUserCol != null && !string.IsNullOrWhiteSpace(insertUserCol.Name))
            {
                return insertUserCol.Name;
            }

            return Constants.NotAvailable;
        }

        public string GetUpdatedDateName()
        {
            var updateDateCol = Columns.FirstOrDefault(c => cfg.UpdateDateFunc(c.Name));
            if (updateDateCol != null && !string.IsNullOrWhiteSpace(updateDateCol.Name))
            {
                return updateDateCol.Name;
            }

            return Constants.NotAvailable;
        }

        public string GetInsertedDateName()
        {
            var insertDateCol = Columns.FirstOrDefault(c => cfg.InsertDateFunc(c.Name));
            if (insertDateCol != null && !string.IsNullOrWhiteSpace(insertDateCol.Name))
            {
                return insertDateCol.Name;
            }

            return Constants.NotAvailable;
        }
    }
}