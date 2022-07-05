using System.Threading.Tasks;
using affolterNET.Data.SessionHandler;

namespace affolterNET.Data.Interfaces.SessionHandler;

public interface IHistorySaver
{
    Task<bool> SaveHistory(string name, string query, string user, string access = "write");

    Task<bool> SaveHistory<TResult>(IQuery<TResult> query);

    EnumHistoryMode HistoryMode { get; }
}