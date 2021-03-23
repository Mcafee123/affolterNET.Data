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
            this.input = input; // .Replace(Environment.NewLine, string.Empty);
        }

        public void Generate(Action<MemberDeclarationSyntax> add)
        {
            var node = SyntaxFactory.ParseMemberDeclaration(input);
            if (node == null)
            {
                throw new InvalidOperationException("could not parse member declaration");
            }
            
            var ws = node.NormalizeWhitespace()!;
            add(ws);
        }
    }
}