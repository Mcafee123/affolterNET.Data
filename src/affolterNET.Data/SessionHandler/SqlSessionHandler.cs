using affolterNET.Data.Interfaces.SessionHandler;

namespace affolterNET.Data.SessionHandler
{
    public class SqlSessionHandler: SqlSessionHandlerBase
    {
        private readonly ISqlSessionFactory _factory;

        public SqlSessionHandler(ISqlSessionFactory factory)
        {
            _factory = factory;
        }
        
        protected override ISqlSession CreateSession()
        {
            return _factory.CreateSession();
        }
    }
}