using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using affolterNET.Data.DtoHelper.CodeGen;
using affolterNET.Data.DtoHelper.Database;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using Xunit.Abstractions;

namespace affolterNET.Data.DtoHelper.Test.CodeGen
{
    public class SelectGeneratorTest
    {
        private readonly ITestOutputHelper _output;

        public SelectGeneratorTest(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public void GenerateOnePkTest()
        {
            var generatorCfg = new GeneratorCfg();
            var tbl = new Table(generatorCfg) { Name = "T_Test", Schema = "dbo" };
            var cols = new List<Column>();
            cols.Add(new Column(generatorCfg) { Name = "TestId", IsPK = true });
            cols.Add(new Column(generatorCfg) { Name = "Bezeichnung" });
            // ReSharper disable once PossibleNullReferenceException
            typeof(Table)
                .GetField("AllColumns",BindingFlags.Instance|BindingFlags.NonPublic)
                .SetValue(tbl, cols);
            var gen = new SelectGenerator(tbl);
            var list = new List<MemberDeclarationSyntax>();
            gen.Generate(mds => list.Add(mds));
            var m = Assert.Single(list) as MethodDeclarationSyntax;
            Assert.NotNull(m);
            Assert.NotNull(m.Body);
            var method = Regex.Replace(m.Body.ToString(), @"\s+", " ", RegexOptions.Multiline);
            _output.WriteLine(method);
            const string expectation = "{ return $\"select top({maxCount}) [TestId], [Bezeichnung] from dbo.T_Test where (@TestId is null or [TestId]=@TestId)\"; }";
            Assert.Equal(expectation, method);
        }
        
        [Fact]
        public void GenerateTwoPkTest()
        {
            var generatorCfg = new GeneratorCfg();
            var tbl = new Table(generatorCfg) { Name = "T_TestOther", Schema = "dbo" };
            var cols = new List<Column>();
            cols.Add(new Column(generatorCfg) { Name = "TestId", IsPK = true });
            cols.Add(new Column(generatorCfg) { Name = "OtherId", IsPK = true });
            cols.Add(new Column(generatorCfg) { Name = "Bezeichnung" });
            // ReSharper disable once PossibleNullReferenceException
            typeof(Table)
                .GetField("AllColumns",BindingFlags.Instance|BindingFlags.NonPublic)
                .SetValue(tbl, cols);
            var gen = new SelectGenerator(tbl);
            var list = new List<MemberDeclarationSyntax>();
            gen.Generate(mds => list.Add(mds));
            var m = Assert.Single(list) as MethodDeclarationSyntax;
            Assert.NotNull(m);
            Assert.NotNull(m.Body);
            var method = Regex.Replace(m.Body.ToString(), @"\s+", " ", RegexOptions.Multiline);
            _output.WriteLine(method);
            const string expectation = "{ return $\"select top({maxCount}) [TestId], [OtherId], [Bezeichnung] from dbo.T_TestOther where (@TestId is null or [TestId]=@TestId) and (@OtherId is null or [OtherId]=@OtherId)\"; }";
            Assert.Equal(expectation, method);
        }
    }
}