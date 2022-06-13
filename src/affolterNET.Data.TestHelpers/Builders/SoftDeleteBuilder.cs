using System;
using System.Collections.Generic;
using System.Data;
using affolterNET.Data.Extensions;
using affolterNET.Data.Interfaces;
using Dapper;

namespace affolterNET.Data.TestHelpers.Builders
{
    public class SoftDeleteBuilder<T> : CrudBase<T> where T : IDtoBase
    {
        private readonly IList<string> updateStatements = new List<string>();

        private string sql = string.Empty;

        public SoftDeleteBuilder(IDbConnection conn, IDbTransaction trsact, IDtoBase dto)
            : base(conn, trsact, dto)
        {
            updateStatements.Add("IstAktiv=@IstAktiv");
            Paras.Add("IstAktiv", 0);
            updateStatements.Add($"{dto.GetUpdatedDateName().EnsureSquareBrackets()}=@{dto.GetUpdatedDateName()}");
            Paras.Add(dto.GetUpdatedDateName(), DateTime.Now);
            updateStatements.Add($"{dto.GetUpdatedUserName().EnsureSquareBrackets()}=@{dto.GetUpdatedUserName()}");
            Paras.Add(dto.GetUpdatedUserName(), $"SoftDeleteBuilder: {TableName}");
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

        public SoftDeleteBuilder<T> WithWhere(string col, object value, bool whereIn = false)
        {
            AddWhere(col, value, whereIn);
            return this;
        }
    }
}