using System;
using System.Collections.Generic;
using System.Data;
using affolterNET.Data.Extensions;
using affolterNET.Data.Interfaces;
using Dapper;

namespace affolterNET.Data.TestHelpers.Builders
{
    public class UpdateBuilder<T> : CrudBase<T> where T : IDtoBase
    {
        private readonly IList<string> updateStatements = new List<string>();

        private string sql = string.Empty;

        public UpdateBuilder(IDbConnection conn, IDbTransaction trsact, IDtoBase dto)
            : base(conn, trsact, dto)
        {
            AddUpdate("LetzteAenderungAm", DateTime.Now);
            AddUpdate("LetzteAenderungDurch", $"UpdateBuilder: {dto.GetTableName()}");
        }

        public UpdateBuilder<T> WithUpdate(string col, object value)
        {
            AddUpdate(col, value);
            return this;
        }

        public UpdateBuilder<T> WithWhere(string col, object value, bool whereIn = false)
        {
            AddWhere(col, value, whereIn);
            return this;
        }

        public UpdateBuilder<T> WithWhereIn(string col, object values)
        {
            return WithWhere(col, values, true);
        }

        public int Execute()
        {
            sql = $"update {TableName}";
            if (updateStatements.Count > 0)
            {
                sql += $" set {string.Join(", ", updateStatements)}";
            }

            if (WhereStatements.Count > 0)
            {
                sql += $" where {string.Join(" and ", WhereStatements)}";
            }

            return Conn.Execute(sql, Paras, Trsact);
        }

        private void AddUpdate(string col, object value)
        {
            var withoutBrackets = $"upd_{col.StripSquareBrackets()}";
            updateStatements.Add($"{col}=@{withoutBrackets}");
            Paras.Add(withoutBrackets, value);
        }
    }
}