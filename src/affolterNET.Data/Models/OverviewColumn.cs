using affolterNET.Data.Models.Filters;

namespace affolterNET.Data.Models
{
    public class OverviewColumn : SqlAttribute
    {
        public OverviewColumn()
        {
        }

        public OverviewColumn(string column, string prefix = "", int? flex = null) : base(column, prefix)
        {
            Flex = flex;
        }

        public int? Flex { get; set; }
    }
}