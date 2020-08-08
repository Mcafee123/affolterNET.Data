using System.Data;

namespace affolterNET.Data.TestHelpers.Interfaces
{
    public interface IConnectionDecorator : IDbConnection
    {
        void RollbackTestTransaction();
    }
}