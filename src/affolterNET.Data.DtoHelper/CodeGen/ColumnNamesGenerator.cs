using System;
using System.Linq;
using affolterNET.Data.DtoHelper.Database;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace affolterNET.Data.DtoHelper.CodeGen
{
    public class ColumnNamesGenerator
    {
        private readonly Table tbl;

        public ColumnNamesGenerator(Table tbl)
        {
            this.tbl = tbl;
        }

        public void Generate(Action<MemberDeclarationSyntax> add)
        {
            var cols = tbl.Columns.Select(c => $"public const string {c.PropertyName} = \"[{c.PropertyName}]\";");
            var allCols = string.Join("\", \"", tbl.Columns.Select(c => c.Name));
            var sg1 = new StringGenerator(
                $@"
                private static readonly List<string> colNames = new List<string> {{ ""{allCols}"" }};
            ");
            sg1.Generate(add);
            var sg2 = new StringGenerator($@"
                public IEnumerable<string> GetColumnNames() => colNames;
            ");
            sg2.Generate(add);
            var sg3 = new StringGenerator($@"
                public static IEnumerable<string> ColNames => colNames;
            ");
            sg3.Generate(add);
            var sg4 = new StringGenerator($@"
                public static class Cols {{
                    {string.Join(Environment.NewLine, cols)}
                }}
            ");
            sg4.Generate(add);
        }
    }
}