namespace affolterNET.Data.Interfaces.SessionHandler
{
    public interface ISqlSessionFactory
    {
        ISqlSession CreateSession();
    }
}