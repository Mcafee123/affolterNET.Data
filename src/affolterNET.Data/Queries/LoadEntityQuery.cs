using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using affolterNET.Data.Interfaces;
using affolterNET.Data.Models.Filters;
using affolterNET.Data.Result;
using Dapper;

namespace affolterNET.Data.Queries
{
    public class LoadEntityQuery<T>: CommandQueryBase<IEnumerable<T>>, IQuery<IEnumerable<T>> where T: class, IViewBase
    {
        public LoadEntityQuery(string userName = "", int maxcount = 1000, string? idName = null): base(userName)
        {
            // command (can work with or without id)
            var dto = Activator.CreateInstance<T>();
            Sql = dto.GetSelectCommand(maxcount);
            if (idName == null && dto is IDtoBase dtobase)
            {
                idName = dtobase.GetIdName();
                AddParam(idName, null!);
            }
        }
        
        public LoadEntityQuery(RootFilter filter, string userName = "", int maxcount = 1000): base(userName)
        {
            // command (can work with or without id)
            var dto = Activator.CreateInstance<T>();
            var sql = dto.GetSelectCommand(maxcount);
            Sql = $"{sql.Substring(0, sql.IndexOf(" where ", StringComparison.InvariantCultureIgnoreCase))} {filter}";
            foreach (var p in filter.GetAllParameters())
            {
                AddParam(p.Key, p.Value);
            }
        }
        
        public LoadEntityQuery(object pkValue, string userName = "", string? idName = null): base(userName)
        {
            // command (can work with or without id)
            var dto = Activator.CreateInstance<T>();
            Sql = dto.GetSelectCommand();
            if (dto is IDtoBase dtobase)
            {
                idName = dtobase.GetIdName();
                AddParam(idName, pkValue);
            }
        }
        
        public override async Task<DataResult<IEnumerable<T>>> ExecuteAsync(IDbConnection connection, IDbTransaction transaction)
        {
            var result = await connection.QueryAsync<T>(Sql, ParamsObject, transaction);
            return new DataResult<IEnumerable<T>>(result);
        }
    }
}