using affolterNET.Data.Models.Filters;
using Xunit;

namespace affolterNET.Data.Test.Models.Filters
{
    public class OrderByTest
    {
        [Theory]
        [InlineData("[Test]", true, "[Test] desc")]
        [InlineData("Test", false, "p.[Test]", "p")]
        public void FromJsonTest(string attribute, bool desc, string expectedString, string prefix = "")
        {
            var orderBy = OrderBy.For(attribute, prefix);
            orderBy.Desc = desc;
            Assert.True(orderBy.WasSet);
            Assert.Equal(expectedString, orderBy.ToString());
        }

        [Theory]
        [InlineData(true, "fa", "Name", "fa", "Name")]
        [InlineData(false, "fa", "Name", "a", "Name")]
        [InlineData(false, "fa", "Name", "fa", "ame")]
        [InlineData(true, "fa", "[Name]", "fa", "[Name]")]
        [InlineData(true, "fa", "Name", "fa", "[Name]")]
        [InlineData(true, "fa", "[Name]", "fa", "Name")]
        [InlineData(false, "fa", "[Name]", "fa", "ame")]
        public void IsValidTests(bool expectedValid, string prefix, string attribute, string metaPrefix, string metaColumn)
        {
            var orderBy = OrderBy.For(attribute, prefix);
            var meta = new SqlAttributes(new SqlAttribute(metaColumn, metaPrefix));
            Assert.Equal(expectedValid, orderBy.IsValid(meta));
        }
    }
}