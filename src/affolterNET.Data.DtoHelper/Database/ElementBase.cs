using System;

namespace affolterNET.Data.DtoHelper.Database
{
    public abstract class ElementBase
    {
        protected string GetIndent(int indent)
        {
            return string.Format("{0,-" + indent + "}", Environment.NewLine);
        }
    }
}