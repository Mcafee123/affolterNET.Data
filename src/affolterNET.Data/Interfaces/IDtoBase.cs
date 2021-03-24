using System;
using System.Data;

namespace affolterNET.Data.Interfaces
{
    public interface IDtoBase
    {
        string GetInsertCommand(bool returnScopeIdentity = false);

        string GetSelectCommand(int maxCount = 1000);

        string GetUpdateCommand();

        string GetDeleteCommand();

        string GetDeleteAllCommand();

        string GetSaveByIdCommand(bool select = true);

        bool IsAutoincrementId();

        void SetId(object id);

        string GetTableName();

        string GetIdName();

        string GetUpdatedUserName();

        string GetInsertedUserName();

        string GetUpdatedDateName();

        string GetInsertedDateName();

        string GetIsActiveName();

        string GetVersionName();

        void SetInsertedUser(string userName);

        void SetInsertedDate(DateTime date);

        void SetUpdatedUser(string userName);

        void SetUpdatedDate(DateTime date);

        void Reload(IDbConnection conn, IDbTransaction trsact);
    }
}