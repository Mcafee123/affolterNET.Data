using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using affolterNET.Data.Result;

namespace affolterNET.Data.Interfaces
{
    public interface IQuery<T>
    {
        IDictionary<string, object> ParamsDict { get; }

        DataResult<T> Execute(IDbConnection connection, IDbTransaction transaction);

        Task<DataResult<T>> ExecuteAsync(IDbConnection connection, IDbTransaction transaction);

        bool ExcludeFromHistory { get; }
        
        bool CheckNotExplicitlySetExcludeFromHistory { get; }

        string? Sql { get;  }

        string UserName { get; }
    }
}