using System.Data;
using affolterNET.Data.TestHelpers.Interfaces;

namespace affolterNET.Data.TestHelpers
{
    public sealed class ConnectionDecorator : IConnectionDecorator
    {
        private readonly IDbConnection _conn;

        private IDbTransaction? _trsact;

        public ConnectionDecorator(IDbConnection conn)
        {
            _conn = conn;
            ConnectionString = "";
        }

        public string ConnectionString { get; set; }

        public int ConnectionTimeout => _conn.ConnectionTimeout;

        public string Database => _conn.Database;

        public ConnectionState State => _conn.State;

        public void Dispose()
        {
            _conn.Dispose();
        }

        public IDbTransaction BeginTransaction()
        {
            return BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            if (_trsact == null)
            {
                var trans = _conn.BeginTransaction(il);
                _trsact = new TransactionDecorator(trans);
            }

            return _trsact;
        }

        public void Close()
        {
            _conn.Close();
        }

        public void ChangeDatabase(string databaseName)
        {
            _conn.ChangeDatabase(databaseName);
        }

        public IDbCommand CreateCommand()
        {
            return _conn.CreateCommand();
        }

        public void Open()
        {
            _conn.Open();
        }

        public void RollbackTestTransaction()
        {
            _trsact?.Rollback();
            _trsact = null;
        }
    }
}