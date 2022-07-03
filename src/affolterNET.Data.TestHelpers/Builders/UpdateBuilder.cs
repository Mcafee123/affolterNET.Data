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
        private readonly IList<string> _updateStatements = new List<string>();

        private string sql = string.Empty;

        public UpdateBuilder(IDbConnection conn, IDbTransaction trsact, IDtoBase dto)
            : base(conn, trsact, dto)
        {
            if (dto.GetUpdatedUserName() != Constants.NotAvailable)
            {
                AddUpdate(dto.GetUpdatedDateName(), DateTime.Now);
                AddUpdate(dto.GetUpdatedUserName(), $"UpdateBuilder: {dto.GetTableName()}");
            }
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
            if (_updateStatements.Count > 0)
            {
                sql += $" set {string.Join(", ", _updateStatements)}";
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
            _updateStatements.Add($"{col}=@{withoutBrackets}");
            Paras.Add(withoutBrackets, value);
        }
    }
}