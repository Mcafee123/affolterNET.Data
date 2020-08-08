using System.Data;
using affolterNET.Data.Interfaces.SessionHandler;

namespace affolterNET.Data.TestHelpers.SessionHandler
{
    public sealed class TestSqlSession : ISqlSession
    {
        public TestSqlSession(IDbConnection cnn, IDbTransaction transaction)
        {
            Connection = cnn;
            Transaction = transaction;
        }

        public void Dispose()
        {
            // do nothing
        }

        public IDbConnection Connection
        {
            get;
        }

        public bool HasTransaction => true;

        public IDbTransaction Transaction
        {
            get;
        }

        public void Begin(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            // do nothing
        }

        public void Commit()
        {
            // do nothing
        }

        public void Rollback()
        {
            // do nothing
        }
    }
}