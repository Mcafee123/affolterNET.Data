using System.Collections.Generic;
using System.Threading.Tasks;
using affolterNET.Data.Models;
using affolterNET.Data.Result;

namespace affolterNET.Data.Interfaces.DataServices
{
    public interface IEntityDataService<T>
    {
        Task<DataResult<IEnumerable<T>>> GetAll(int maxcount = 1000);
        Task<T> GetById(object pkValue, string userName = "", string? param = null);
        Task<SaveInfo> Save(T dto);
        Task<T> SaveAndReload(T dto, string? userName = null);
        Task<bool> DeleteAll();
        Task<bool> DeleteById(object pkValue, byte[] timestamp, string? param = null);
    }
}