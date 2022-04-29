using System.Text.RegularExpressions;

namespace affolterNET.Data.DtoHelper.Extensions;

public static class StringExtensions
{
    public static string CleanMemberName(this string input, bool isField = false)
    {
        input = input.Replace("ä", "ae");
        input = input.Replace("ö", "oe");
        input = input.Replace("ü", "ue");
        input = Regex.Replace(input, "[^a-zA-Z0-9]", "");
        if (isField)
        {
            input = $"_{input.Substring(0, 1).ToLower()}{input.Substring(1)}";
        }
        else
        {
            input = $"{input.Substring(0, 1).ToUpper()}{input.Substring(1)}";   
        }

        return input;
    }
}