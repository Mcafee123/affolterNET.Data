using System;
using System.Data;
using System.Threading.Tasks;
using affolterNET.Data.Interfaces;
using affolterNET.Data.Interfaces.SessionHandler;
using affolterNET.Data.Result;

namespace affolterNET.Data.TestHelpers.SessionHandler
{
    public class TestSqlSessionHandler : ISqlSessionHandler
    {
        private readonly IDbConnection _cnn;
        private readonly Guid _id;
        private readonly IDbTransaction? _transaction;
        private TestSqlSession? _session;

        public TestSqlSessionHandler(IDbConnection cnn, IDbTransaction? transaction)
        {
            _cnn = cnn;
            _transaction = transaction;
            _id = Guid.NewGuid();
        }

        public DataResult<int> Execute(ICommand command, IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            return ExecuteAsync(command, isolationLevel).GetAwaiter().GetResult();
        }

        public async Task<DataResult<int>> ExecuteAsync(
            ICommand command,
            IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            return await QueryAsync(command, isolationLevel);
        }

        public DataResult<TResult> Query<TResult>(
            IQuery<TResult> query,
            IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            return QueryAsync(query, isolationLevel).GetAwaiter().GetResult();
        }

        public async Task<DataResult<TResult>> QueryAsync<TResult>(
            IQuery<TResult> query,
            IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            if (_session == null)
            {
                throw new InvalidOperationException("Db Session was null");
            }

            return await query.ExecuteAsync(_session.Connection, _session.Transaction);
        }

        public ISqlSession CreateSqlSession()
        {
            if (_session == null)
            {
                _session = new TestSqlSession(_cnn, _transaction);
            }

            return _session;
        }

        public Task<DataResult<TResult>> QueryMultipleAsync<TResult>(Func<Task<DataResult<TResult>>> dbAction, IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"TestSqlSessionHandler: {_id}";
        }
    }
}