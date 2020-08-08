using System.Collections.Generic;
using affolterNET.Data.DtoHelper.Database;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace affolterNET.Data.DtoHelper.CodeGen
{
    public class ClassGenerator
    {
        private readonly List<MemberDeclarationSyntax> list;

        private readonly Table tbl;

        public ClassGenerator(Table tbl)
        {
            this.tbl = tbl;
            list = new List<MemberDeclarationSyntax>();
        }

        public ClassDeclarationSyntax Generate(ClassDeclarationSyntax classDeclaration)
        {
            // Table Name
            var sgTableName = new StringGenerator($"public const string TABLE_NAME = \"[{tbl.Schema}].[{tbl.Name}]\";");
            sgTableName.Generate(Add);

            // Properties für Attribute
            foreach (var col in tbl.Columns)
            {
                var sgP = new StringGenerator(col.WriteProperty(0));
                sgP.Generate(Add);
            }

            var sgColumnNames = new ColumnNamesGenerator(tbl);
            sgColumnNames.Generate(Add);

            if (!tbl.IsView)
            {
                // IsAutoincrement
                var sgAutoInc = new StringGenerator(tbl.GetPrimaryKeyColumn().WriteIsAutoincrementId(0));
                sgAutoInc.Generate(Add);
            }

            // GetTableName
            var sgTn = new StringGenerator("public string GetTableName() { return TABLE_NAME; }");
            sgTn.Generate(Add);

            // SelectGenerator
            var sgSelect = new SelectGenerator(tbl);
            sgSelect.Generate(Add);

            // InsertGenerator
            var sgInsert = new InsertGenerator(tbl);
            sgInsert.Generate(Add);

            // UpdateGenerator
            var sgUpdate = new UpdateGenerator(tbl);
            sgUpdate.Generate(Add);

            // DeleteGenerator
            var sgDelete = new DeleteGenerator(tbl);
            sgDelete.Generate(Add);

            if (!tbl.IsView)
            {
                // Refresh()
                var sgRefresh = new RefreshGenerator(tbl);
                sgRefresh.Generate(Add);

                // GetIdName
                var sgIdName = new StringGenerator(
                    $"public string GetIdName() {{ return \"{tbl.GetPrimaryKeyColumn().Name}\"; }}");
                sgIdName.Generate(Add);

                // SetId
                var sgSetId = new StringGenerator(tbl.GetPrimaryKeyColumn().WriteSetId(0));
                sgSetId.Generate(Add);
            }

            // GetVersionName
            var sgVersionName =
                new StringGenerator($"public string GetVersionName() {{ return \"{tbl.GetVersionName()}\"; }}");
            sgVersionName.Generate(Add);

            // GetIsActiveName
            var sgGetIsActive = new StringGenerator(
                $"public string GetIsActiveName() {{ return \"{tbl.GetIsActiveName()}\"; }}");
            sgGetIsActive.Generate(Add);

            // SetIsActive
            var isActive = tbl.GetIsActiveName();
            isActive = isActive == tbl.NotAvailable ? string.Empty : $"this.{isActive} = isActive;";
            var sgSetIsActive = new StringGenerator($"public void SetIsActive(bool isActive) {{ {isActive} }}");
            sgSetIsActive.Generate(Add);

            // GetUpdatedUserName
            var sgUpdateUser = new StringGenerator(
                $"public string GetUpdatedUserName() {{ return \"{tbl.GetUpdatedUserName()}\"; }}");
            sgUpdateUser.Generate(Add);

            // SetUpdatedUser
            var updatedUser = tbl.GetUpdatedUserName();
            updatedUser = updatedUser == tbl.NotAvailable ? string.Empty : $"this.{updatedUser} = userName;";
            var sgSetUpdatedUser =
                new StringGenerator($"public void SetUpdatedUser(string userName) {{ {updatedUser} }}");
            sgSetUpdatedUser.Generate(Add);

            // GetInsertedUserName
            var sgInsertUser = new StringGenerator(
                $"public string GetInsertedUserName() {{ return \"{tbl.GetInsertedUserName()}\"; }}");
            sgInsertUser.Generate(Add);

            // SetInsertedUser
            var insertedUser = tbl.GetInsertedUserName();
            insertedUser = insertedUser == tbl.NotAvailable ? string.Empty : $"this.{insertedUser} = userName;";
            var sgSetInsertedUser =
                new StringGenerator($"public void SetInsertedUser(string userName) {{ {insertedUser} }}");
            sgSetInsertedUser.Generate(Add);

            // GetUpdatedDateName
            var sgUpdateDate = new StringGenerator(
                $"public string GetUpdatedDateName() {{ return \"{tbl.GetUpdatedDateName()}\"; }}");
            sgUpdateDate.Generate(Add);

            // SetUpdatedDate
            var updatedDate = tbl.GetUpdatedDateName();
            updatedDate = updatedDate == tbl.NotAvailable ? string.Empty : $"this.{updatedDate} = date;";
            var sgSetUpdatedDate =
                new StringGenerator($"public void SetUpdatedDate(DateTime date) {{ {updatedDate} }}");
            sgSetUpdatedDate.Generate(Add);

            // GetInsertedDateName
            var sgInsertDate = new StringGenerator(
                $"public string GetInsertedDateName() {{ return \"{tbl.GetInsertedDateName()}\"; }}");
            sgInsertDate.Generate(Add);

            // SetInsertedDate
            var insertedDate = tbl.GetInsertedDateName();
            insertedDate = insertedDate == tbl.NotAvailable ? string.Empty : $"this.{insertedDate} = date;";
            var sgSetInsertedDate =
                new StringGenerator($"public void SetInsertedDate(DateTime date) {{ {insertedDate} }}");
            sgSetInsertedDate.Generate(Add);

            // ToString()
            var sgToString = new StringGenerator($"public override string ToString() {{ return $\"{tbl}\";}}");
            sgToString.Generate(Add);

            classDeclaration = classDeclaration.AddMembers(list.ToArray());

            return classDeclaration;
        }

        private void Add(MemberDeclarationSyntax mds)
        {
            list.Add(mds);
        }
    }
}