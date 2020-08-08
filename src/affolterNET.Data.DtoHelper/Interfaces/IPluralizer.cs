namespace affolterNET.Data.DtoHelper.Interfaces
{
    public interface IPluralizer
    {
        string Pluralize(string name);

        string Singularize(string name);
    }
}