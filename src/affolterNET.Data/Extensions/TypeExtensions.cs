using System;
using System.Linq;

namespace affolterNET.Data.Extensions;

public static class TypeExtensions
{
    public static string GetGenericArgsFriendlyName(this Type type)
    {
        var name = type.Name;
        if (type.GenericTypeArguments.Length > 0)
        {
            if (name.EndsWith("`1") && name.Length > 2)
            {
                name = name.Substring(0, name.Length - 2);
            }

            var typeArgs = type.GenericTypeArguments.Select(a => a.Name);
            name = $"{name}<{string.Join(", ", typeArgs)}>";
        }

        return name;
    }
}