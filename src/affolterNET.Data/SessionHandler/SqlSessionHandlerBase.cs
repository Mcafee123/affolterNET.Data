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
    public abstract class SqlSessionHandlerBase : ISqlSessionHandler
    {
        private readonly Guid _id;
        protected ISqlSession? Session;

        protected SqlSessionHandlerBase()
        {
            _id = Guid.NewGuid();
        }

        protected abstract ISqlSession CreateSession();
        
        protected abstract void SaveHistory<TResult>(IQuery<TResult> query);

        public ISqlSession CreateSqlSession()
        {
            Session = CreateSession();
            return Session;
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
            Log.Verbose("{Query}",query.ToString());
            return await QueryMultipleAsync(() =>
            {
                var result = query.ExecuteAsync(Session!.Connection, Session.Transaction!);
                SaveHistory(query);
                return result;
            }, isolationLevel);
        }

        public async Task<DataResult<TResult>> QueryMultipleAsync<TResult>(
            Func<Task<DataResult<TResult>>> dbAction,
            IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            var hasSession = HasSession();
            if (!hasSession)
            {
                Session = CreateSqlSession();
            }

            var hasTransaction = Session!.HasTransaction;
            if (!hasTransaction)
            {
                Session.Begin(isolationLevel);
            }

            try
            {
                var result = await dbAction();
                if (!hasTransaction)
                {
                    if (result.IsSuccessful)
                    {
                        Session.Commit();
                    }
                    else
                    {
                        Session.Rollback();
                    }
                }

                return result;
            }
            catch
            {
                if (!hasTransaction)
                {
                    Session.Transaction!.Rollback();
                }

                throw;
            }
            finally
            {
                if (!hasSession)
                {
                    Session.Dispose();
                    Session = null;
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

        protected bool HasSession()
        {
            return Session?.Connection != null;
        }
    }
}