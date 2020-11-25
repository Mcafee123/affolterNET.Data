using System;
using System.Data;
using System.Threading.Tasks;
using affolterNET.Data.Interfaces;
using affolterNET.Data.Interfaces.SessionHandler;
using affolterNET.Data.Result;
using Serilog;

namespace affolterNET.Data.SessionHandler
{
// ReSharper disable once ClassNeverInstantiated.Global
    public class SqlSessionHandler : ISqlSessionHandler
    {
        private readonly ISqlSessionFactory _factory;
        private readonly Guid _id;
        private ISqlSession? _session;

        public SqlSessionHandler(ISqlSessionFactory factory)
        {
            _factory = factory;
            _id = Guid.NewGuid();
        }

        public ISqlSession CreateSqlSession()
        {
            _session = _factory.CreateSession();
            return _session;
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
            Log.Verbose(query.ToString());
            return await QueryMultipleAsync(() => query.ExecuteAsync(_session!.Connection, _session.Transaction!), isolationLevel);
        }

        public async Task<DataResult<TResult>> QueryMultipleAsync<TResult>(
            Func<Task<DataResult<TResult>>> dbAction,
            IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            var hasSession = HasSession();
            if (!hasSession)
            {
                _session = CreateSqlSession();
            }

            var hasTransaction = _session!.HasTransaction;
            if (!hasTransaction)
            {
                _session.Begin(isolationLevel);
            }

            try
            {
                var result = await dbAction();
                if (!hasTransaction)
                {
                    if (result.IsSuccessful)
                    {
                        _session.Commit();
                    }
                    else
                    {
                        _session.Rollback();
                    }
                }

                return result;
            }
            catch
            {
                if (!hasTransaction)
                {
                    _session.Transaction!.Rollback();
                }

                throw;
            }
            finally
            {
                if (!hasSession)
                {
                    _session.Dispose();
                    _session = null;
                }
            }
        }

        public async Task<DataResult<int>> ExecuteMultipleAsync(
            Func<Task<DataResult<int>>> dbAction,
            IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            return await QueryMultipleAsync(dbAction, isolationLevel);
        }

        public override string ToString()
        {
            return $"SqlSessionHandler: {_id}";
        }

        private bool HasSession()
        {
            return _session?.Connection != null;
        }
    }
}