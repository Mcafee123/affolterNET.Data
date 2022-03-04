using System;
using System.Linq;
using affolterNET.Data.DtoHelper.Database;
using affolterNET.Data.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace affolterNET.Data.DtoHelper.CodeGen
{
    public class SelectGenerator
    {
        private readonly Table tbl;

        public SelectGenerator(Table tbl)
        {
            this.tbl = tbl;
        }

        public void Generate(Action<MemberDeclarationSyntax> add)
        {
            // Select
            var columns = tbl.AllColumns.Select(c => c.Name);
            var keys = tbl.GetPrimaryKeyColumns().ToList();
            var selectWhere = string.Empty;
            if (keys.Count > 0)
            {
                selectWhere = " where " + string.Join(" and ", keys.Select(WhereStatement));
            }
            var sgSelect = new StringGenerator(
                $@"
                public string GetSelectCommand(int maxCount = 1000, params string[] excludedColumns)
                {{
                    var cols = ""{columns.JoinCols()}"".GetColumns(excludedColumns);
                    return $""select top({{maxCount}}) {{cols.JoinCols()}} from {tbl.Schema}.{tbl.Name}{selectWhere}"";
                }}");
            sgSelect.Generate(add);
        }

        private string WhereStatement(Column col)
        {
            return $"(@{col.Name} is null or {col.Name!.EnsureSquareBrackets()}=@{col.Name})";
        }
    }
}