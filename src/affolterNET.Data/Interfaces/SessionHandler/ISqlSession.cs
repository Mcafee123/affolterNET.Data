using System;
using System.Data;

namespace affolterNET.Data.Interfaces.SessionHandler
{
    public interface ISqlSession : IDisposable
    {
        IDbConnection Connection { get; }

        bool HasTransaction { get; }

        IDbTransaction Transaction { get; }

        void Begin(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted);

        void Commit();

        void Rollback();
    }
}