using System;
using System.Collections.Generic;
using System.Linq;
using affolterNET.Data.DtoHelper.Database;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace affolterNET.Data.DtoHelper.CodeGen
{
    public class UpdateGenerator
    {
        private readonly Table tbl;

        public UpdateGenerator(Table tbl)
        {
            this.tbl = tbl;
        }

        public void Generate(Action<MemberDeclarationSyntax> add)
        {
            var updateWhere = string.Empty;
            var versionWhere = string.Empty;
            var cols = new List<string>();
            foreach (var col in tbl.AllColumns.Where(c => !c.Ignore))
            {
                if (col.IsVersionCol())
                {
                    versionWhere = $" and {col.Name}=@{col.Name}";
                }

                if (col.IsPK)
                {
                    updateWhere = string.Format("where {0}=@{0}", col.Name);
                }
                else
                {
                    if (!col.IsVersionCol() && !col.IsInsertTriggerField() && !col.IsActiveCol())
                    {
                        cols.Add(string.Format("{0}=@{0}", col.Name));
                    }
                }
            }

            var sql = $"update {tbl.Schema}.{tbl.Name} set {string.Join(", ", cols)} {updateWhere}{versionWhere}";
            var inner = tbl.IsView
                ? "throw new InvalidOperationException(\"no updates on views\");"
                : $"return \"{sql}\";";
            var sg = new StringGenerator(
                $@"
                public string GetUpdateCommand()
                {{
                    {inner}
                }}
            ");
            sg.Generate(add);
        }
    }
}