using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using affolterNET.Data.Interfaces.SessionHandler;
using affolterNET.Data.TestHelpers.Interfaces;
using affolterNET.Data.TestHelpers.SessionHandler;
using Microsoft.Extensions.Configuration;

namespace affolterNET.Data.TestHelpers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public abstract class DbFixture : IDisposable
    {
        private readonly string _connStringKey;
        private readonly string? _userSecretsId;
        private string? _connString;
        private ISqlSessionHandler? _handler;
        private ITransactionDecorator? _transaction;
        
        public IConnectionDecorator? Connection { get; private set; }

        public IDbTransaction? Transaction => _transaction?.WrappedTransaction;

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

        protected DbFixture(string connStringKey = "CONNSTRING", string? userSecretsId = null)
        {
            _connStringKey = connStringKey;
            _userSecretsId = userSecretsId;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void EndTest()
        {
            Connection?.RollbackTestTransaction();
            _transaction?.Dispose();
            _transaction = null!;
        }

        public void StartTest()
        {
            if (Connection == null)
            {
                _connString = GetConnString();
                var cn = new SqlConnection(_connString);
                Connection = new ConnectionDecorator(cn);
                Connection.Open();
            }

            _transaction = Connection.BeginTransaction() as ITransactionDecorator;
            _handler = new TestSqlSessionHandler(Connection, _transaction!.WrappedTransaction);
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

        protected virtual string GetConnString()
        {
            if (_connString == null)
            {
                var builder = new ConfigurationBuilder();
                if (!string.IsNullOrWhiteSpace(_userSecretsId))
                {
                    builder.AddUserSecrets(_userSecretsId);
                }
                var config = builder.AddEnvironmentVariables().Build();
                _connString = config.GetValue<string>(_connStringKey);
                if (string.IsNullOrWhiteSpace(_connString))
                {
                    var msg = $@"ConnString empty. For local development, 
add '{_connStringKey}' with the connection to the database to user secrets.";
                    throw new ConfigurationErrorsException(msg);
                }
            }

            return _connString;
        }
    }
}