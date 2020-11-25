using System.Collections.Generic;
using System.Data;
using System.Linq;
using affolterNET.Data.Interfaces;
using Dapper;

namespace affolterNET.Data.TestHelpers.Builders
{
    public class SelectBuilder<T> : CrudBase<T>
        where T : IDtoBase
    {
        private string sql = string.Empty;

        public SelectBuilder(IDbConnection conn, IDbTransaction trsact, IDtoBase dto)
            : base(conn, trsact, dto)
        {
        }

        public SelectBuilder<T> WithWhere(string col, object value, bool whereIn = false)
        {
            AddWhere(col, value, whereIn);
            return this;
        }

        public SelectBuilder<T> WithWhereIn(string col, object values)
        {
            return WithWhere(col, values, true);
        }

        public T ExecuteSingle()
        {
            sql = $"select top(1) * from {TableName}";
            return RunExecute().FirstOrDefault()!;
        }

        public IEnumerable<T> Execute()
        {
            sql = $"select * from {TableName}";
            return RunExecute();
        }

        private IEnumerable<T> RunExecute()
        {
            if (WhereStatements.Count > 0)
            {
                sql += $" where {string.Join(" and ", WhereStatements)}";
            }

            var list = Conn.Query<T>(sql, Paras, Trsact);
            return list;
        }
    }
}