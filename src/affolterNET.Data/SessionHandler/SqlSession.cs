using System;
using System.Data;
using System.Data.SqlClient;
using affolterNET.Data.Interfaces.SessionHandler;

namespace affolterNET.Data.SessionHandler
{
    internal sealed class SqlSession : ISqlSession
    {
        private bool _disposed;

        public SqlSession(string connectionString)
        {
            Connection = new SqlConnection(connectionString);
            Connection.Open();
        }

        public bool HasTransaction => Transaction != null;

        public IDbConnection Connection { get; private set; }

        public IDbTransaction Transaction { get; private set; }

        public void Begin(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            Transaction = Connection.BeginTransaction(isolationLevel);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Commit()
        {
            Transaction.Commit();
        }

        public void Rollback()
        {
            Transaction.Rollback();
        }

        // ReSharper disable once StyleCop.SA1201
        ~SqlSession()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Transaction Dispose (wenn noch da...)
                    Transaction?.Dispose();
                    Transaction = null;

                    // Connection Dispose
                    Connection?.Close();
                    Connection?.Dispose();
                    Connection = null;
                }

                _disposed = true;
            }
        }
    }
}