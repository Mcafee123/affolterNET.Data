using System;
using System.Data.SqlClient;
using affolterNET.Data.Interfaces.SessionHandler;
using affolterNET.Data.TestHelpers.Interfaces;
using affolterNET.Data.TestHelpers.SessionHandler;

namespace affolterNET.Data.TestHelpers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public abstract class DbFixture : IDisposable
    {
        private string _connString;
        private ISqlSessionHandler _handler;

        public IConnectionDecorator Connection { get; private set; }

        public ITransactionDecorator Transaction { get; set; }

        public ISqlSessionHandler SqlSessionHandler
        {
            get
            {
                if (_handler == null)
                {
                    throw new InvalidOperationException("Handler not ready");
                }

                return _handler;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void EndTest()
        {
            Connection.RollbackTestTransaction();
            Transaction.Dispose();
            Transaction = null;
        }

        public void StartTest()
        {
            if (Connection == null)
            {
                _connString = GetConnString();
                var cn = new SqlConnection(_connString);
                Connection = new ConnectionDecorator(cn);
                Connection.Open();
                _handler = new TestSqlSessionHandler(Connection, Transaction);
            }

            Transaction = Connection.BeginTransaction() as ITransactionDecorator;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Connection != null)
                {
                    Connection.Dispose();
                    Connection = null;
                }
            }
        }

        protected abstract string GetConnString();
    }
}