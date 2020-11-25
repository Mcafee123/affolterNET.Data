using System;
using System.Collections.Generic;
using affolterNET.Data.DtoHelper.Database;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace affolterNET.Data.DtoHelper.CodeGen
{
    public class ClassesGenerator
    {
        private readonly GeneratorCfg cfg;

        public ClassesGenerator(GeneratorCfg cfg)
        {
            this.cfg = cfg;
        }

        public NamespaceDeclarationSyntax Generate(NamespaceDeclarationSyntax ns, Tables tables)
        {
            var cds = new List<MemberDeclarationSyntax>();

            // dtos
            foreach (var tbl in tables)
            {
                if (string.IsNullOrWhiteSpace(tbl.ObjectName))
                {
                    throw new InvalidOperationException($"{nameof(tbl.ObjectName)} was empty");
                }

                // class
                var classDeclaration = SyntaxFactory.ClassDeclaration(tbl.ObjectName);
                classDeclaration = classDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                if (tbl.IsView)
                {
                    foreach (var viewBaseType in cfg.ViewBaseTypes)
                    {
                        classDeclaration = classDeclaration.AddBaseListTypes(
                            SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(viewBaseType)));
                    }
                }
                else
                {
                    foreach (var baseType in cfg.BaseTypes)
                    {
                        classDeclaration = classDeclaration.AddBaseListTypes(
                            SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseType)));
                    }
                }

                var cd = new ClassGenerator(tbl);
                classDeclaration = cd.Generate(classDeclaration);
                cds.Add(classDeclaration);
            }

            return ns.AddMembers(cds.ToArray());
        }
    }
}