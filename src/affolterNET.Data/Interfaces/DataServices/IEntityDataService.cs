using System.Collections.Generic;
using System.Threading.Tasks;
using affolterNET.Data.Result;

namespace affolterNET.Data.Interfaces.DataServices
{
    public interface IEntityDataService<T>
    {
        Task<DataResult<IEnumerable<T>>> GetAll(int maxcount = 1000);
        Task<T> GetById(object pkValue, string? param = null);
        Task<string> Save(T dto);
        Task<T> SaveAndReload(T dto);
        Task<bool> DeleteAll();
        Task<bool> DeleteById(object pkValue, string? param = null);
    }
}