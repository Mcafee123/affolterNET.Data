using System.Collections.Generic;

namespace affolterNET.Data.Models.Filters
{
    public class RootFilter : Filter
    {
        public RootFilter() { }

        public RootFilter(string column, string prefix = "") : base(column, prefix)
        {
        }

        public override string ToString()
        {
            // enumerate all filters to be able to distinguish the different sql parameters
            SetIndex(1);

            var filter = base.ToString();
            if (string.IsNullOrWhiteSpace(filter))
            {
                return string.Empty;
            }

            return $"where {filter}";
        }

        public IDictionary<string, object> GetAllParameters()
        {
            return GetParameters();
        }

        public bool IsValid()
        {
            return IsFilterValid();
        }

        public List<SqlAttribute> GetAttributes()
        {
            return GetFilterAttributes(new List<SqlAttribute>());
        }
    }
}