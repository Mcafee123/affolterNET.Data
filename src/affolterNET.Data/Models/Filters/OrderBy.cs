
namespace affolterNET.Data.Models.Filters
{
    public class OrderBy
    {
        public OrderBy()
        {
        }

        private OrderBy(string attribute, string prefix = "")
        {
            Attribute = new SqlAttribute(attribute, prefix);
        }

        public SqlAttribute? Attribute { get; set; }

        public bool Desc { get; set; }

        public bool WasSet => !string.IsNullOrWhiteSpace(Attribute?.Column);

        public bool IsValid(SqlAttributes attributes)
        {
            if (Attribute != null && !attributes.Contains(Attribute))
            {
                return false;
            }

            return true;
        }

        public static OrderBy For(string attribute, string prefix = "")
        {
            return new(attribute, prefix);
        }

        public override string ToString()
        {
            var desc = Desc ? " desc" : "";
            return $"{Attribute}{desc}";
        }
    }
}