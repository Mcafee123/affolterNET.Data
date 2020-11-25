using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using affolterNET.Data.DtoHelper.CodeGen;
using affolterNET.Data.DtoHelper.Database;
using affolterNET.Data.DtoHelper.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace affolterNET.Data.DtoHelper
{
    public class Generator
    {
        private readonly GeneratorCfg cfg;

        private readonly ClassesGenerator cg;

        private readonly IFileHandler fh;

        private readonly TablesLoader tl;

        private NamespaceDeclarationSyntax? ns;

        private CompilationUnitSyntax? root;

        public Generator(GeneratorCfg cfg, IFileHandler fh)
        {
            this.cfg = cfg;
            this.fh = fh;
            tl = new TablesLoader(this.cfg);
            cg = new ClassesGenerator(cfg);
        }

        public async Task Generate()
        {
            var tr = LoadTables();

            await SetRootAndNamespace();

            FillStatics(tr);

            FillDtos(tr.Tables);

            WriteTargetFile();
        }

        private TablesResultat LoadTables()
        {
            var res = tl.LoadTables();
            if ((res.Error != null) || (res.Ex != null))
            {
                throw new InvalidOperationException(res.Error, res.Ex);
            }

            return res;
        }

        private async Task SetRootAndNamespace()
        {
            // Parse the code into a SyntaxTree.
            var tree = CSharpSyntaxTree.ParseText(string.Empty);

            // Get the root CompilationUnitSyntax.
            root = await tree.GetRootAsync().ConfigureAwait(false) as CompilationUnitSyntax;

            if (root == null)
            {
                throw new InvalidOperationException("Root des Codefiles war null");
            }

            // usings
            var usings = cfg.Usings.Select(
                u =>
                {
                    var us = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(u));
                    if (u.IndexOf("DataAnnotations", StringComparison.Ordinal) > -1)
                    {
                        us = us.WithAlias(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName("Da")));
                    }

                    return us;
                }).ToArray();
            root = root.AddUsings(usings);

            // namespace
            ns = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(cfg.Namespace));

            // comments
            var comm = cfg.Comments.Select(SyntaxFactory.Comment).ToArray();
            ns = ns.WithLeadingTrivia(comm);
        }

        private void FillStatics(TablesResultat tr)
        {
            var staticsBuilder = new StringBuilder();
            var staticsViewsBuilder = new StringBuilder();
            foreach (var tbl in tr.Tables)
            {
                var ifstring = $"if (typeof({tbl.ObjectName}) == typeof(T)) {{ return new {tbl.ObjectName}(); }}";
                if (tbl.IsView)
                {
                    staticsViewsBuilder.Append(ifstring);
                }
                else
                {
                    staticsBuilder.Append(ifstring);
                }
            }

            var statics = staticsBuilder.ToString();
            var staticsViews = staticsViewsBuilder.ToString();

            // tablefactory
            var getTableString =
                $"public IDtoBase Get<T>() where T : IDtoBase {{ {statics} throw new InvalidOperationException(); }}";
            var dbString = $"public class DtoFactory: IDtoFactory {{{getTableString}}}";
            var sgStaticTableFactory = new StringGenerator(dbString);
            var listTables = new List<MemberDeclarationSyntax>();
            sgStaticTableFactory.Generate(mds => listTables.Add(mds));
            ns = ns!.AddMembers(listTables.ToArray());

            // viewfactory
            var getViewString =
                $"public IViewBase Get<T>() where T : IViewBase {{ {staticsViews} throw new InvalidOperationException(); }}";
            var viewString = $"public class ViewFactory: IViewFactory {{{getViewString}}}";
            var sgStaticViewFactory = new StringGenerator(viewString);
            var listViews = new List<MemberDeclarationSyntax>();
            sgStaticViewFactory.Generate(mds => listViews.Add(mds));
            ns = ns.AddMembers(listViews.ToArray());
        }

        private void FillDtos(Tables tables)
        {
            // classes
            ns = cg.Generate(ns!, tables);
            root = root!.AddMembers(ns);
        }

        private void WriteTargetFile()
        {
            // Write the new file.
            fh.WriteCode(root!.NormalizeWhitespace().ToFullString());
        }
    }
}