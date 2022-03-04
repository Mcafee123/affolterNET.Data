using System;
using System.Collections.Generic;
using System.Linq;
using affolterNET.Data.DtoHelper.Database;
using affolterNET.Data.Extensions;
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
            }
            var columns = tbl.AllColumns
                .Where(
                    c => !c.Ignore && !c.IsPkWithAutoincrement() && !c.IsVersionCol() &&
                         !c.IsInsertTriggerField() && !c.IsActiveCol())
                .Select(c => c.Name!.StripSquareBrackets()).ToList();
            
            var content = $@"
                var cols = ""{columns.JoinCols()}"".GetColumns(excludedColumns);
                return $""update {tbl.Schema}.{tbl.Name} set {{cols.JoinForUpdate()}} {updateWhere}{versionWhere}"";
            ";
            var inner = tbl.IsView
                ? "throw new InvalidOperationException(\"no updates on views\");"
                : content;
            var sg = new StringGenerator(
                $@"
                public string GetUpdateCommand(params string[] excludedColumns)
                {{
                    {inner}
                }}
            ");
            sg.Generate(add);
        }
    }
}