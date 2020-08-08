using System.Collections.Generic;
using System.Linq;
using affolterNET.Data.Interfaces.Models.Filters;

namespace affolterNET.Data.Models.Filters
{
    public class TextFilter : IFilter
    {
        public const string Equal = "equals";
        public const string NotEqual = "notEqual";
        public const string Contains = "contains";
        public const string NotContains = "notContains";
        public const string StartsWith = "startsWith";
        public const string EndsWith = "endsWith";
        private readonly SqlAttribute _attribute;
        private readonly string _comparer;
        private readonly int _index;
        private readonly Dictionary<string, string> _dict;

        public TextFilter(SqlAttribute attribute, string comparer, int index)
        {
            _attribute = attribute;
            _comparer = comparer;
            _index = index;
            _dict = new Dictionary<string, string>
            {
                { Equal, "{0} = {1}" },
                { NotEqual, "({0} is null or {0} <> {1})" },
                { Contains, "{0} like '%' + {1} + '%'" },
                { NotContains, "({0} is null or {0} not like '%' + {1} + '%')" },
                { StartsWith, "{0} like {1} + '%'" },
                { EndsWith, "{0} like '%' + {1}" }
            };
        }

        public bool IsValid()
        {
            return _dict.Keys.ToList().IndexOf(_comparer) > -1;
        }

        public string GetSql()
        {
            var attr = _attribute.ToSqlParamIdentifier(_index);
            return string.Format(_dict[_comparer], _attribute, attr);
        }
    }
}