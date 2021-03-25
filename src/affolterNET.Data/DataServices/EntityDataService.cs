using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using affolterNET.Data.Commands;
using affolterNET.Data.Interfaces;
using affolterNET.Data.Interfaces.DataServices;
using affolterNET.Data.Interfaces.SessionHandler;
using affolterNET.Data.Models;
using affolterNET.Data.Queries;
using affolterNET.Data.Result;
using Microsoft.Extensions.Logging;

namespace affolterNET.Data.DataServices
{
    public class EntityDataService<T>: IEntityDataService<T>  where T: class, IDtoBase
    {
        private readonly ISqlSessionHandler _sessionHandler;
        private readonly ILogger<EntityDataService<T>> _logger;

        public EntityDataService(ISqlSessionHandler sessionHandler, ILogger<EntityDataService<T>> logger)
        {
            _sessionHandler = sessionHandler;
            _logger = logger;
        }
        
        public async Task<DataResult<IEnumerable<T>>> GetAll(int maxcount = 1000)
        {
            var qry = new LoadEntityCommand<T>(maxcount);
            var result = await _sessionHandler.QueryAsync(qry);
            if (!result.IsSuccessful)
            {
                result.LogError(_logger, nameof(GetAll));
            }

            return result;
        }
        
        public async Task<T> GetById(object pkValue, string? param = null)
        {
            var qry = new LoadEntityCommand<T>(pkValue, param);
            var result = await _sessionHandler.QueryAsync(qry);
            if (!result.IsSuccessful)
            {
                result.LogError(_logger, nameof(GetById));
            }

            return result.Data.First();
        }

        public async Task<SaveInfo> Save(T dto)
        {
            var cmd = new SaveEntityCommand<T>(dto);
            var result = await _sessionHandler.QueryAsync(cmd);
            if (!result.IsSuccessful)
            {
                result.LogError(_logger, nameof(Save));
            }
            return result.Data;
        }

        public async Task<T> SaveAndReload(T dto)
        {
            var cmd = new SaveAndReloadEntityCommand<T>(dto);
            var result = await _sessionHandler.QueryAsync(cmd);
            if (!result.IsSuccessful)
            {
                result.LogError(_logger, nameof(Save));
            }
            return result.Data;
        }
        
        public async Task<bool> DeleteAll()
        {
            var cmd = new DeleteEntityCommand<T>();
            var result = await _sessionHandler.QueryAsync(cmd);
            if (!result.IsSuccessful)
            {
                result.LogError(_logger, nameof(DeleteAll));
            }

            return result.Data;
        }
        
        public async Task<bool> DeleteById(object pkValue, string? param = null)
        {
            var cmd = new DeleteEntityCommand<T>(pkValue, param);
            var result = await _sessionHandler.QueryAsync(cmd);
            if (!result.IsSuccessful)
            {
                result.LogError(_logger, nameof(DeleteById));
            }

            return result.Data;
        }
    }
}