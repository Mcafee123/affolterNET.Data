using affolterNET.Data.Extensions;

namespace affolterNET.Data.Models.Filters
{
    public class SqlAttribute
    {
        private string _column;

        public SqlAttribute() { }

        public SqlAttribute(string column, string prefix = "")
        {
            Column = column;
            Prefix = prefix;
        }

        public string Prefix { get; set; }

        public string Column
        {
            get => _column;
            set => _column = value?.StripSquareBrackets();
        }

        public override string ToString()
        {
            var prefix = string.IsNullOrWhiteSpace(Prefix) ? "" : $"{Prefix}.";
            return $"{prefix}{Column.EnsureSquareBrackets()}";
        }

        public string ToSqlParamIdentifier(int index)
        {
            return $"@{ToParam(index)}";
        }

        public string ToParam(int index)
        {
            return $"{Prefix}{index}{Column}";
        }
    }
}