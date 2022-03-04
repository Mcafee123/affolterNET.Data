using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using affolterNET.Data.Extensions;
using affolterNET.Data.Interfaces;
using affolterNET.Data.Result;
using Dapper;

namespace affolterNET.Data.Queries
{
    public class LoadEntityQuery<T>: CommandQueryBase<IEnumerable<T>>, IQuery<IEnumerable<T>> where T: class, IDtoBase
    {
        public LoadEntityQuery(int maxcount = 1000, string? idName = null)
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
        
        public LoadEntityQuery(int maxcount = 1000, params Tuple<string, object>[] filters)
        {
            // command (can work with or without id)
            var dto = Activator.CreateInstance<T>();
            Sql = dto.GetSelectCommand(maxcount);
            foreach (var filter in filters)
            {
                //dto.
                var colname = filter.Item1.StripSquareBrackets();
                Sql += $" and {colname}=@{colname}";
                AddParam(colname, filter.Item2);
            }
            AddParam(dto.GetIdName(), null!);
        }
        
        public LoadEntityQuery(object pkValue, string? idName = null)
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
            var result = await connection.QueryAsync<T>(Sql, ParamsObject, transaction);
            return new DataResult<IEnumerable<T>>(result);
        }
    }
}