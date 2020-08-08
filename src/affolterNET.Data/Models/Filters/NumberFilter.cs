using System.Collections.Generic;
using System.Linq;
using affolterNET.Data.Interfaces.Models.Filters;

namespace affolterNET.Data.Models.Filters
{
    public class NumberFilter : IFilter
    {
        public const string Equal = "equals";
        public const string NotEqual = "notEqual";
        public const string LessThan = "lessThan";
        public const string LessThanOrEqual = "lessThanOrEqual";
        public const string GreaterThan = "greaterThan";
        public const string GreaterThanOrEqual = "greaterThanOrEqual";
        public const string InRange = "inRange";
        private readonly SqlAttribute _attribute;
        private readonly string _comparer;
        private readonly int _index;
        private readonly Dictionary<string, string> _dict;

        public NumberFilter(SqlAttribute attribute, string comparer, int index)
        {
            _attribute = attribute;
            _comparer = comparer;
            _index = index;
            _dict = new Dictionary<string, string>
            {
                { Equal, "{0} = {1}" },
                { NotEqual, "{0} <> {1}" },
                { LessThan, "{0} < {1}" },
                { LessThanOrEqual, "{0} <= {1}" },
                { GreaterThan, "{0} > {1}" },
                { GreaterThanOrEqual, "{0} >= {1}" },
                { InRange, "{0} between {1} and {2}" }
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