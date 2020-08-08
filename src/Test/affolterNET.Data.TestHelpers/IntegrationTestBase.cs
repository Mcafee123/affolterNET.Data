﻿using System;
using affolterNET.Data.Interfaces;
using affolterNET.Data.Interfaces.SessionHandler;
using affolterNET.Data.TestHelpers.Builders;

namespace affolterNET.Data.TestHelpers
{
    public abstract class IntegrationTestBase : IDisposable
    {
        private readonly IDtoFactory _dtoFactory;

        private readonly DateTime _startTime;
        protected readonly DbFixture Fixture;

        private DbOperations _ops;

        protected IntegrationTestBase(DbFixture dbFixture, IDtoFactory dtoFactory)
        {
            Fixture = dbFixture;
            _dtoFactory = dtoFactory;
            _startTime = DateTime.Now;

            dbFixture.StartTest();
        }

        protected ISqlSessionHandler TestSqlSessionHandler => Fixture.SqlSessionHandler;

        public DbOperations TestOperations
        {
            get
            {
                if (_ops == null)
                {
                    _ops = new DbOperations(
                        Fixture.Connection,
                        Fixture.Transaction.WrappedTransaction,
                        _dtoFactory);
                }

                return _ops;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public CommandQueryBuilder<TResult> CQB<TResult>(bool checkParameters = true)
        {
            return new CommandQueryBuilder<TResult>(Fixture, _dtoFactory, checkParameters);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Fixture.EndTest();

                var endTime = DateTime.Now;

                // ReSharper disable once UnusedVariable
                var t = endTime - _startTime;

                // Schöne Ausgabe
                // var prettyString = t.ToPrettyString(1, UnitStringRepresentation.Long, TimeSpanUnit.Minutes, TimeSpanUnit.Milliseconds);
                // prettyString = $"{Environment.NewLine}Duration: {prettyString}{Environment.NewLine}";
                // Console.WriteLine(prettyString);
                // Debug.WriteLine(prettyString);
            }
        }
    }
}