using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace affolterNET.Data.Models.Filters
{
    public class SqlAttributes : IEnumerable<SqlAttribute>
    {
        private readonly List<SqlAttribute> _list = new List<SqlAttribute>();

        public SqlAttributes(params SqlAttribute[] attributes)
        {
            AddRange(attributes);
        }

        public IEnumerator<SqlAttribute> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(SqlAttribute attribute)
        {
            return Contains(attribute.Column, attribute.Prefix);
        }

        public bool Contains(string column, string? prefix)
        {
            return _list.Any(
                a => string.Equals(a.Prefix, prefix, StringComparison.CurrentCultureIgnoreCase) &&
                     string.Equals(a.Column, column, StringComparison.CurrentCultureIgnoreCase));
        }

        public void AddRange(IEnumerable<SqlAttribute> attributes)
        {
            _list.AddRange(attributes);
        }
    }
}