using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using affolterNET.Data.Extensions;
using affolterNET.Data.Interfaces;

namespace affolterNET.Data.TestHelpers.Builders
{
    public abstract class CrudBase<T>
        where T : IDtoBase
    {
        protected readonly IDbConnection Conn;

        protected readonly IDtoBase Dto;

        protected readonly IDictionary<string, object> Paras = new ExpandoObject()!;

        protected readonly IDbTransaction Trsact;

        protected readonly IList<string> WhereStatements = new List<string>();

        protected CrudBase(IDbConnection conn, IDbTransaction trsact, IDtoBase dto)
        {
            Conn = conn;
            Trsact = trsact;
            Dto = dto;
            TableName = Dto.GetTableName();
        }

        protected string TableName { get; }

        protected void AddWhere(string col, object value, bool whereIn)
        {
            var symbol = whereIn ? " in " : "=";
            var withoutBrackets = col.StripSquareBrackets();
            WhereStatements.Add($"{col}{symbol}@{withoutBrackets}");
            Paras.Add(withoutBrackets, value);
        }
    }
}