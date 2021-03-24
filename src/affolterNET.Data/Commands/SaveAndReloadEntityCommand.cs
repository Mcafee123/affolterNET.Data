using System.Data;
using System.Threading.Tasks;
using affolterNET.Data.Interfaces;
using affolterNET.Data.Result;
using Dapper;

namespace affolterNET.Data.Commands
{
    public class SaveAndReloadEntityCommand<T> : CommandQueryBase<T>, IQuery<T> where T : class, IDtoBase
    {
        public SaveAndReloadEntityCommand(T dto)
        {
            Sql = dto.GetSaveByIdCommand();
            AddParams(dto);
        }

        public override async Task<DataResult<T>> ExecuteAsync(IDbConnection connection, IDbTransaction transaction)
        {
            var result = await connection.QueryFirstOrDefaultAsync<T>(Sql, ParamsObject, transaction);
            return new DataResult<T>(result);
        }
    }
}