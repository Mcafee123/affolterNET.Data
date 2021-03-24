using System;
using System.Data;
using System.Threading.Tasks;
using affolterNET.Data.Interfaces;
using affolterNET.Data.Result;
using Dapper;

namespace affolterNET.Data.Commands
{
    public class SaveEntityCommand<T>: CommandQueryBase<string>, IQuery<string> where T: class, IDtoBase
    {
        public SaveEntityCommand(T dto)
        {
            Sql = dto.GetSaveByIdCommand(false);
            AddParams(dto);
        }

        public override async Task<DataResult<string>> ExecuteAsync(IDbConnection connection, IDbTransaction transaction)
        {
            var result = await connection.QueryFirstOrDefaultAsync<string>(Sql, ParamsObject, transaction);
            return new DataResult<string>(result);
        }
    }
}