using System;
using System.Linq;
using affolterNET.Data.DtoHelper.Database;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace affolterNET.Data.DtoHelper.CodeGen
{
    public class DeleteGenerator
    {
        private readonly Table tbl;

        public DeleteGenerator(Table tbl)
        {
            this.tbl = tbl;
        }

        public void Generate(Action<MemberDeclarationSyntax> add)
        {
            var pkCol = tbl.AllColumns.FirstOrDefault(c => c.IsPK);
            var versionCol = tbl.AllColumns.FirstOrDefault(c => c.IsVersionCol());
            string sql;
            var sqlAll = $"return \"delete from {tbl.Schema}.{tbl.Name}";
            if (pkCol == null)
            {
                sql = "throw new InvalidOperationException(\"Kein Primary Key\");";
            }
            else
            {
                var updateWhere = string.Format(" where {0}=@{0}", pkCol.Name);
                var versionWhere = string.Empty;
                if (versionCol != null)
                {
                    versionWhere = $" and {versionCol.Name}=@{versionCol.Name}";
                }

                sql = $"{sqlAll}{updateWhere}{versionWhere}\"";
            }

            var sg = new StringGenerator(
                $@"
                public string GetDeleteCommand() {{
                    {sql};
                }}
            ");

            sg.Generate(add);
            
            var sgAll = new StringGenerator(
                $@"
                public string GetDeleteAllCommand() {{
                    {sqlAll}"";
                }}
            ");
            
            sgAll.Generate(add);
        }
    }
}