using System.Collections.Generic;
using affolterNET.Data.Interfaces.Models.Filters;

namespace affolterNET.Data.Models.Filters
{
    public class Filter
    {
        protected int Index = -1;

        public Filter()
        {
            FilterType = "text";
            Comparer = "equals";
        }

        public Filter(string column, string prefix = "") : this()
        {
            Attribute = new SqlAttribute(column, prefix);
        }

        public SqlAttribute? Attribute { get; set; }

        public string Comparer { get; set; }

        public object? Value { get; set; }

        public string FilterType { get; set; }

        public bool UseAnd { get; set; } = true;

        public List<Filter> Filters { get; set; } = new List<Filter>();

        public bool WasSet => Attribute != null;

        public void AddFilter(string column, object? value, string prefix = "")
        {
            Filters.Add(new Filter(column, prefix) { Value = value });
        }

        public void AddFilter(Filter filter)
        {
            Filters.Add(filter);
        }

        private IFilter InternalFilter
        {
            get
            {
                switch (FilterType.ToLower())
                {
                    case "number":
                        return new NumberFilter(Attribute!, Comparer, Index);

                    case "date":
                        return new DateFilter(Attribute!, Comparer, Index);

                    default:
                        return new TextFilter(Attribute!, Comparer, Index);
                }
            }
        }

        protected bool IsFilterValid()
        {
            if (Filters.Count > 0)
            {
                foreach (var filter in Filters)
                {
                    if (!filter.IsFilterValid())
                    {
                        return false;
                    }
                }

                return true;
            }

            if (WasSet)
            {
                return InternalFilter.IsValid();
            }

            return true;
        }

        protected int SetIndex(int index)
        {
            Index = index;
            foreach (var f in Filters)
            {
                index += 1;
                index = f.SetIndex(index);
            }

            return index;
        }

        public override string ToString()
        {
            if (Filters.Count > 0)
            {
                var op = UseAnd ? "and" : "or";
                return $"({string.Join($" {op} ", Filters)})";
            }

            if (WasSet)
            {
                return InternalFilter.GetSql();
            }

            return string.Empty;
        }

        protected IDictionary<string, object> GetParameters()
        {
            var list = new Dictionary<string, object>();
            if (WasSet)
            {
                list.Add(Attribute!.ToParam(Index), Value!);
            }
            else
            {
                foreach (var filter in Filters)
                {
                    foreach (var kvp in filter.GetParameters())
                    {
                        list.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            return list;
        }

        protected List<SqlAttribute> GetFilterAttributes(List<SqlAttribute> attributes)
        {
            if (WasSet)
            {
                attributes.Add(Attribute!);
            }
            else
            {
                foreach (var filter in Filters)
                {
                    filter.GetFilterAttributes(attributes);
                }
            }

            return attributes;
        }
    }
}