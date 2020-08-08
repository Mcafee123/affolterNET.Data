namespace affolterNET.Data.Interfaces.Models.Filters
{
    public interface IFilter
    {
        bool IsValid();

        string GetSql();
    }
}