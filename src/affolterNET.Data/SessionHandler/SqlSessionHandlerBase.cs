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

        /// <summary>
        /// Exceptions will be caught
        /// </summary>
        /// <param name="command"></param>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        public DataResult<int> Execute(ICommand command, IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            return ExecuteAsync(command, isolationLevel).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Exceptions will be caught
        /// </summary>
        /// <param name="command"></param>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        public async Task<DataResult<int>> ExecuteAsync(
            ICommand command,
            IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            return await QueryAsync(command, isolationLevel);
        }

        /// <summary>
        /// Exceptions will be caught
        /// </summary>
        /// <param name="query"></param>
        /// <param name="isolationLevel"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public DataResult<TResult> Query<TResult>(
            IQuery<TResult> query,
            IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            return QueryAsync(query, isolationLevel).GetAwaiter().GetResult();
        }

        /// <summary>
        /// exceptions will be caught
        /// </summary>
        /// <param name="query"></param>
        /// <param name="isolationLevel"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<DataResult<TResult>> QueryAsync<TResult>(
            IQuery<TResult> query,
            IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            DataResult<TResult>? result = null;
            try
            {
                result = await QueryMultipleAsync(() =>
                {
                    var res = query.ExecuteAsync(Session!.Connection, Session.Transaction!);
                    SaveHistory(query);
                    return res;
                }, isolationLevel);
                return result;
            }
            catch (Exception ex)
            {
                result = new DataResult<TResult>(ex);
            }
            finally
            {
                result!.SqlCommand = query.ToString();
            }

            return result;
        }

        /// <summary>
        /// this throws exceptions
        /// </summary>
        /// <param name="dbAction"></param>
        /// <param name="isolationLevel"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
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

        /// <summary>
        /// This throws exceptions
        /// </summary>
        /// <param name="dbAction"></param>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
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