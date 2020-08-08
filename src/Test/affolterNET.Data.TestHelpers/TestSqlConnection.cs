using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace affolterNET.Data.TestHelpers
{
    public sealed class TestSqlConnection : DbConnection
    {
        private readonly SqlConnection _conn;

        private DbTransaction _trsact;

        public TestSqlConnection(SqlConnection conn)
        {
            _conn = conn;
            ConnectionString = _conn.ConnectionString;
        }

        public override string ConnectionString
        {
            get => _conn.ConnectionString;
            set => _conn.ConnectionString = value;
        }

        public override string Database => _conn.Database;

        public override ConnectionState State => _conn.State;

        public override string DataSource => _conn.DataSource;

        public override string ServerVersion => _conn.ServerVersion;

        protected override DbTransaction BeginDbTransaction(IsolationLevel il)
        {
            if (_trsact == null)
            {
                var trans = _conn.BeginTransaction(il);
                _trsact = new TestSqlTransaction(trans);
            }

            return _trsact;
        }

        public override void ChangeDatabase(string databaseName)
        {
            _conn.ChangeDatabase(databaseName);
        }

        public override void Close()
        {
            _conn.Close();
        }

        public override void Open()
        {
            _conn.Open();
        }

        protected override DbCommand CreateDbCommand()
        {
            return _conn.CreateCommand();
        }
    }
}