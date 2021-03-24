using System;
using System.Data;
using System.Threading.Tasks;
using affolterNET.Data.Interfaces;
using affolterNET.Data.Result;
using Dapper;

namespace affolterNET.Data.Commands
{
    public class DeleteEntityCommand<T>: CommandQueryBase<bool>, IQuery<bool> where T: class, IDtoBase
    {
        public DeleteEntityCommand()
        {
            var dto = Activator.CreateInstance<T>();
            Sql = dto.GetDeleteAllCommand();
        }
        
        public DeleteEntityCommand(object pkValue, string? param = null)
        {
            var dto = Activator.CreateInstance<T>();
            Sql = dto.GetDeleteCommand();
            if (param == null)
            {
                param = dto.GetIdName();
            }
            AddParam(param, pkValue);
        }
        
        public override async Task<DataResult<bool>> ExecuteAsync(IDbConnection connection, IDbTransaction transaction)
        {
            var result = await connection.ExecuteAsync(Sql, ParamsObject, transaction);
            return new DataResult<bool>(result > 0);
        }
    }
}