using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace affolterNET.Data.DtoHelper.CodeGen
{
    public class StringGenerator
    {
        private readonly string input;

        public StringGenerator(string input)
        {
            this.input = input.Replace(Environment.NewLine, string.Empty);
        }

        public void Generate(Action<MemberDeclarationSyntax> add)
        {
            var node = CSharpSyntaxTree.ParseText(input);
            var root = node.GetCompilationUnitRoot();
            if (root.Members.Count < 1)
            {
                throw new InvalidOperationException("Kein Rootnode gefunden beim Parsing");
            }

            foreach (var r in root.Members)
            {
                var ws = r.NormalizeWhitespace()!;
                add(ws);
            }
        }
    }
}