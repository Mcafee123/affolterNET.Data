using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using affolterNET.Data.Interfaces;
using affolterNET.Data.Result;
using Dapper;

namespace affolterNET.Data.Queries
{
    [Obsolete("Please use LoadEntityQuery")]
    public class LoadEntityCommand<T>: CommandQueryBase<IEnumerable<T>>, IQuery<IEnumerable<T>> where T: class, IDtoBase
    {
        public LoadEntityCommand(int maxcount = 1000, string? idName = null)
        {
            // command (can work with or without id)
            var dto = Activator.CreateInstance<T>();
            Sql = dto.GetSelectCommand(maxcount);
            if (idName == null)
            {
                idName = dto.GetIdName();
            }
            AddParam(idName, null!);
        }
        
        public LoadEntityCommand(object pkValue, string? idName = null)
        {
            // command (can work with or without id)
            var dto = Activator.CreateInstance<T>();
            Sql = dto.GetSelectCommand();
            if (idName == null)
            {
                idName = dto.GetIdName();
            }
            AddParam(idName, pkValue);
        }
        
        public override async Task<DataResult<IEnumerable<T>>> ExecuteAsync(IDbConnection connection, IDbTransaction transaction)
        {
            var iEnum = await connection.QueryAsync<T>(Sql, ParamsObject, transaction);
            var result = new DataResult<IEnumerable<T>>(iEnum)
            {
                SqlCommand = ToString()
            };
            return result;
        }
    }
}