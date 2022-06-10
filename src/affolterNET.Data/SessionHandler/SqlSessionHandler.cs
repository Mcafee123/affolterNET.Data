using affolterNET.Data.Interfaces;
using affolterNET.Data.Interfaces.SessionHandler;

namespace affolterNET.Data.SessionHandler
{
    public class SqlSessionHandler: SqlSessionHandlerBase
    {
        private readonly ISqlSessionFactory _factory;
        private readonly IHistorySaver _historySaver;

        public SqlSessionHandler(ISqlSessionFactory factory, IHistorySaver historySaver)
        {
            _factory = factory;
            _historySaver = historySaver;
        }
        
        protected override ISqlSession CreateSession()
        {
            return _factory.CreateSession();
        }

        protected override void SaveHistory<TResult>(IQuery<TResult> query)
        {
            _historySaver.SaveHistory(query).GetAwaiter().GetResult();
        }
    }
}