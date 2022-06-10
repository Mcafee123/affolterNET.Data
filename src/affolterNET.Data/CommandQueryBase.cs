using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using affolterNET.Data.Extensions;
using affolterNET.Data.Models;
using affolterNET.Data.Result;
using Dapper;

namespace affolterNET.Data
{
    public abstract class CommandQueryBase<TResult>
    {
        public const string ScopeIdentity = "select scope_identity() as id";
        public const string LastChangedDate = "LastChangedDate";

        protected CommandQueryBase(bool? excludeFromHistory = null)
        {
            Params = new ExpandoObject();
            if (excludeFromHistory == null)
            {
                CheckNotExplicitlySetExcludeFromHistory = true;
                excludeFromHistory = GetType()!.Namespace!.IndexOf("Commands", StringComparison.CurrentCultureIgnoreCase) == -1;
            }

            ExcludeFromHistory = excludeFromHistory.Value;
        }

        public bool CheckNotExplicitlySetExcludeFromHistory { get; }

        public bool ExcludeFromHistory { get; }

        protected dynamic Params { get; }

        public IDictionary<string, object> ParamsDict => Params;

        protected object ParamsObject => Params;

        public string? Sql { get; protected set; }

        protected void AddParams<T>(T paramsObject, Func<string, bool>? predicate = null)
        {
            if (paramsObject == null)
            {
                throw new ArgumentNullException(nameof(paramsObject), "object cannot be null");
            }

            foreach (var property in paramsObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (predicate == null || predicate(property.Name))
                {
                    var val = property.GetValue(paramsObject);
                    AddParam(property.Name, val!);   
                }
            }
        }

        protected void AddParam(string propertyName, object propertyValue)
        {
            var pn = propertyName.StripSquareBrackets();
            if (ParamsDict.ContainsKey(pn))
            {
                ParamsDict[pn] = propertyValue;
            }
            else
            {
                ParamsDict.Add(pn, propertyValue);
            }
        }

        public virtual DataResult<TResult> Execute(IDbConnection connection, IDbTransaction transaction)
        {
            return ExecuteAsync(connection, transaction).GetAwaiter().GetResult();
        }

        public abstract Task<DataResult<TResult>> ExecuteAsync(IDbConnection connection, IDbTransaction transaction);

        protected async Task<DataResult<IEnumerable<SaveInfo>>> ExecuteSaveInfo(IDbConnection connection,
            IDbTransaction transaction)
        {
            var reader = await connection.QueryMultipleAsync(Sql, ParamsObject, transaction);
            try
            {
                return await TransformSaveInfo(reader);
            }
            finally
            {
                reader.EndQueryMultiple();
            }
        }

        protected async Task<DataResult<IEnumerable<SaveInfo>>> TransformSaveInfo(SqlMapper.GridReader reader)
        {
            var results = new List<SaveInfo>();
            do
            {
                var info = await reader.ReadFirstOrDefaultAsync<SaveInfo>();
                results.Add(info);
            } while (!reader.IsConsumed);

            return new DataResult<IEnumerable<SaveInfo>>(results);
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(
                "-- WIP: Falls die Variablen nicht richtig ausgegeben werden, bitte eLog2.Basis.Daten.Queries.CommandQueryBase.SchreibeWert erweitern");
            sb.AppendLine();
            if (ParamsDict != null)
            {
                foreach (var key in ParamsDict.Keys)
                {
                    var val = ParamsDict[key];
                    var def =
                        "nvarchar(100) -- Parameter Typ konnte nicht evaluiert werden (Wert=null 0der kein passender SqlType)";
                    var propertyType = val == null ? def : SqlType(val.GetType(), def);
                    sb.AppendLine($"declare @{key} {propertyType};");
                    var wert = SchreibeWert(propertyType, val ?? "null");
                    sb.AppendLine($"set @{key} = {wert};");
                    sb.Append(Environment.NewLine);
                }
            }

            sb.Append(Environment.NewLine);
            sb.Append(Sql);
            return sb.ToString();
        }

        private string SchreibeWert(string propertyType, object val)
        {
            if (val == null)
            {
                return "null";
            }

            switch (propertyType)
            {
                case "nvarchar(100)":
                    return $"'{val}'";
                case "int":
                    var i = val as int?;
                    return i?.ToString() ?? "null";
                case "bit":
                    var v = val as bool?;
                    return v.HasValue ? v.Value ? "1" : "0" : "null";
                case "binary":
                    // hex wert bauen
                    var hex = BitConverter.ToString((byte[]) val);
                    hex = $"0x{hex.Replace("-", string.Empty)}";
                    return hex;
                case "datetime":
                    var dt = val as DateTime?;
                    return $"'{dt?.ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
            }

            return $"{val}";
        }

        private string SqlType(Type csharpType, string def)
        {
            var underlying = Nullable.GetUnderlyingType(csharpType);
            if (underlying != null)
            {
                // nullable
                csharpType = underlying;
            }

            switch (csharpType.Name.ToLower())
            {
                case "string":
                    return "nvarchar(100)";
                case "long":
                    return "bigint";
                case "short":
                    return "smallint";
                case "int16":
                case "int32":
                    return "int";
                case "guid":
                    return "uniqueidentifier";
                case "datetime":
                    return "datetime";
                case "float":
                case "double":
                    return "double";
                case "decimal":
                    return "decimal";
                case "tinyint":
                    return "byte";
                case "bool":
                case "boolean":
                    return "bit";
                case "byte[]":
                    return "binary";
                case "Microsoft.SqlServer.Types.SqlGeography":
                    return "geography";
                case "Microsoft.SqlServer.Types.SqlGeometry":
                    return "geometry";
            }

            Console.WriteLine($"type {csharpType.Name.ToLower()} ist nicht implementiert, gebe default zurück");
            return def;
        }
    }
}