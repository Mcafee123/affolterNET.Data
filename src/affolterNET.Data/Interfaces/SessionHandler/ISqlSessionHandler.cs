using System;
using System.Data;
using System.Threading.Tasks;
using affolterNET.Data.Result;

namespace affolterNET.Data.Interfaces.SessionHandler
{
    public interface ISqlSessionHandler
    {
        DataResult<int> Execute(ICommand command, IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted);

        Task<DataResult<int>> ExecuteAsync(
            ICommand command,
            IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted);

        DataResult<TResult> Query<TResult>(
            IQuery<TResult> query,
            IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted);

        Task<DataResult<TResult>> QueryAsync<TResult>(
            IQuery<TResult> query,
            IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted);

        ISqlSession CreateSqlSession();

        Task<DataResult<TResult>> QueryMultipleAsync<TResult>(
            Func<Task<DataResult<TResult>>> dbAction,
            IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted);
    }
}