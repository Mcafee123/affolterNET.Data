using System;
using System.Linq;
using affolterNET.Data.DtoHelper.Database;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace affolterNET.Data.DtoHelper.CodeGen
{
    public class InsertGenerator
    {
        private readonly Table tbl;

        public InsertGenerator(Table tbl)
        {
            this.tbl = tbl;
        }

        public void Generate(Action<MemberDeclarationSyntax> add)
        {
            var cols = tbl.AllColumns
                .Where(
                    c => !c.Ignore && !c.IsPkWithAutoincrement() && !c.IsVersionCol() &&
                         !c.IsUpdateTriggerField(true) && !c.IsActiveCol())
                .Select(c => c.Name).ToList();
            var sg = new StringGenerator(
                $@"
                public string GetInsertCommand(bool returnScopeIdentity = false) {{
                    var sql = ""insert into {tbl.Schema}.{tbl.Name} ({string.Join(", ", cols)}) values (@{string.Join(", @", cols)})"";
                    if (returnScopeIdentity) {{
                        sql += ""; select scope_identity() as id;"";
                    }}
                    return sql;
                }}
            ");
            sg.Generate(add);
        }
    }
}