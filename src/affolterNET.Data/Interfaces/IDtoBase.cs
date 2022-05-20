using System;
using System.Collections.Generic;
using System.Data;

namespace affolterNET.Data.Interfaces
{
    public interface IDtoBase: IViewBase
    {
        string GetInsertCommand(bool returnScopeIdentity = false, params string[] excludedColumns);

        string GetUpdateCommand(params string[] excludedColumns);

        string GetDeleteCommand();

        string GetDeleteAllCommand();

        string GetSaveByIdCommand(bool select = false, params string[] excludedColumns);

        bool IsAutoincrementId();

        void SetId(object id);

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

        IEnumerable<string> GetColumnNames();
    }
}