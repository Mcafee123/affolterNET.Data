using System.Data;
using System.Threading.Tasks;
using affolterNET.Data.Interfaces;
using affolterNET.Data.Models;
using affolterNET.Data.Result;
using Dapper;

namespace affolterNET.Data.Commands
{
    public class SaveEntityCommand<T> : CommandQueryBase<SaveInfo>, IQuery<SaveInfo> where T : class, IDtoBase
    {
        public SaveEntityCommand(T dto)
        {
            Sql = dto.GetSaveByIdCommand();
            AddParams(dto);
        }

        public override async Task<DataResult<SaveInfo>> ExecuteAsync(IDbConnection connection,
            IDbTransaction transaction)
        {
            var result = await connection.QueryFirstOrDefaultAsync<SaveInfo>(Sql, ParamsObject, transaction);
            return new DataResult<SaveInfo>(result);
        }
    }
}