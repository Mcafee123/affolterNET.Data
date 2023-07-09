using System;
using System.Data;
using affolterNET.Data.Interfaces.SessionHandler;
using Dapper;

namespace affolterNET.Data.SessionHandler
{
    public class SqlSessionFactory : ISqlSessionFactory
    {
        private readonly string _connectionString;

        public SqlSessionFactory(string connectionString)
        {
            _connectionString = connectionString;
            SqlMapper.AddTypeMap(typeof(DateOnly), DbType.Date, true);
            SqlMapper.AddTypeMap(typeof(DateOnly?), DbType.Date, true);
        }

        public ISqlSession CreateSession()
        {
            return new SqlSession(_connectionString);
        }
    }
}