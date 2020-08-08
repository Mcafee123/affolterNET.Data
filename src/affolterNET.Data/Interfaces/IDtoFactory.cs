namespace affolterNET.Data.Interfaces
{
    public interface IDtoFactory
    {
        IDtoBase Get<T>()
            where T : IDtoBase;
    }
}