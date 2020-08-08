using affolterNET.Data.Interfaces.SessionHandler;

namespace affolterNET.Data.SessionHandler
{
    public class SqlSessionFactory : ISqlSessionFactory
    {
        private readonly string _connectionString;

        public SqlSessionFactory(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public ISqlSession CreateSession()
        {
            return new SqlSession(_connectionString);
        }
    }
}