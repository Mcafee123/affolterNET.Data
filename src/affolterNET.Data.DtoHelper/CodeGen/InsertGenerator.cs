using System;
using System.Linq;
using affolterNET.Data.DtoHelper.Database;
using affolterNET.Data.Extensions;
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
                public string GetInsertCommand(bool returnScopeIdentity = false, params string[] excludedColumns) {{
                    var cols = ""{cols.JoinCols()}"".GetColumns(excludedColumns);
                    var sql = $""insert into {tbl.Schema}.{tbl.Name} ({{cols.JoinCols()}}) values ({{cols.JoinCols(true)}})"";
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