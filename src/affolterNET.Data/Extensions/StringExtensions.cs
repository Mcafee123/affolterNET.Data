namespace affolterNET.Data.Extensions
{
    public static class StringExtensions
    {
        public static string StripSquareBrackets(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            if (input.StartsWith("["))
            {
                input = input.Substring(1);
            }

            if (input.EndsWith("]"))
            {
                input = input.Substring(0, input.Length - 1);
            }

            return input;
        }

        public static string EnsureSquareBrackets(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            if (!input.StartsWith("["))
            {
                input = $"[{input}";
            }

            if (!input.EndsWith("]"))
            {
                input = $"{input}]";
            }

            return input;
        }
    }
}