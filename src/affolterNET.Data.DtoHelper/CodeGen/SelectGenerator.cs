using System;
using System.Linq;
using affolterNET.Data.DtoHelper.Database;
using affolterNET.Data.Extensions;
using Microsoft.CodeAnalysis;
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
            var sgSelect = new StringGenerator(
                $@"
                public string GetSelectCommand(int maxCount = 1000)
                {{
                    return {SelectCommand()};
                }}");
            sgSelect.Generate(add);
        }

        private string Statement(Column col)
        {
            return $"(@{col.Name} is null or {col.Name!.EnsureSquareBrackets()}=@{col.Name})";
        }

        private string SelectCommand()
        {
            var keys = tbl.GetPrimaryKeyColumns().ToList();
            var selectWhere = string.Empty;
            if (keys.Count > 0)
            {
                selectWhere = " where " + string.Join(" and ", keys.Select(Statement));
            }
            return
                $"$\"select top({{maxCount}}) {string.Join(", ", tbl.AllColumns.Select(c => c.Name!.EnsureSquareBrackets()))} from {tbl.Schema}.{tbl.Name}{selectWhere}\"";
        }
    }
}