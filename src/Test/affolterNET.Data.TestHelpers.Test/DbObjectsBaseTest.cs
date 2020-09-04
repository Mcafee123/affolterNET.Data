using System;
using System.Linq;
using Xunit;
using Xunit.Sdk;

namespace affolterNET.Data.TestHelpers.Test
{
    public class DbObjectsBaseTest
    {
        [Fact]
        public void Get_DifferentObjects_SameName_Test()
        {
            var testee = new DbObjects();
            testee.Create1("a");
            Assert.Throws<XunitException>(() => testee.Create2("a"));
        }
        
        [Fact]
        public void Get_SameObjects_SameName_Test()
        {
            var testee = new DbObjects();
            testee.Create1("a", "erster");
            testee.Create1("a", "zweiter");
            var result = testee.Get<Obj1>("a");
            Assert.Equal("erster", result.Name);
        }

        [Fact]
        public void GetAllTest()
        {
            var testee = new DbObjects();
            
            testee.Create1("a");
            testee.Create1("b");
            testee.Create1("c");
            
            testee.Create2("d");
            testee.Create2("e");
            testee.Create2("f");
            
            testee.Create3("g");
            testee.Create3("h");
            testee.Create3("i");

            var a = testee.GetAll<Obj2>().ToList();
            Assert.NotNull(a);
            Assert.Equal(3, a.Count);
            Assert.Contains("d", a.Select(o => o.Name));
            Assert.Contains("e", a.Select(o => o.Name));
            Assert.Contains("f", a.Select(o => o.Name));
        }
    }

    public class Obj1
    {
        public string Name { get; set; }
    }

    public class Obj2
    {
        public string Name { get; set; }
    }

    public class Obj3
    {
        public string Name { get; set; }
    }

    public class DbObjects : DbObjectsBase
    {
        public void Create1(string name, string objname = null)
        {
            objname ??= name;
            GetSet(() => new Obj1 { Name = objname }, name);
        }
        
        public void Create2(string name, string objname = null)
        {
            objname ??= name;
            GetSet(() => new Obj2 { Name = objname }, name);
        }
        
        public void Create3(string name, string objname = null)
        {
            objname ??= name;
            GetSet(() => new Obj3 { Name = objname }, name);
        }
    }
}