using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using affolterNET.Data.Interfaces;
using affolterNET.Data.Result;
using affolterNET.Data.TestHelpers.Helpers;
using affolterNET.Data.TestHelpers.Interfaces;
using Xunit;

namespace affolterNET.Data.TestHelpers.Builders
{
    public class CommandQueryBuilder<TResult>
    {
        private readonly AssertHelper _assertHelper;

        private readonly bool _checkParameters;

        private readonly DbOperations _dbOperations;

        private Func<DbOperations, IQuery<TResult>> _arrange;

        private Action<DbOperations> _arrangeSimple;

        public CommandQueryBuilder(DbFixture dbFixture, IDtoFactory dtoFactory, bool checkParameters = true)
        {
            Connection = dbFixture.Connection;
            Transaction = dbFixture.Transaction;
            WrappedTransaction = ((ITransactionDecorator)Transaction).WrappedTransaction;
            _dbOperations = new DbOperations(Connection, WrappedTransaction, dtoFactory);
            _assertHelper = new AssertHelper(_dbOperations, dtoFactory);
            this._checkParameters = checkParameters;
        }

        public IConnectionDecorator Connection { get; }

        public IDbTransaction Transaction { get; }

        public IDbTransaction WrappedTransaction { get; }

        public void Rollback()
        {
            Connection.RollbackTestTransaction();
            if (!_assertHelper.InputChecked && _checkParameters)
            {
                Assert.True(false, "Input Parameter wurden nicht überprüft");
            }
        }

        [DebuggerStepThrough]
        public CommandQueryBuilder<TResult> Arrange(Func<DbOperations, IQuery<TResult>> arr)
        {
            _arrange = arr;
            return this;
        }

        [DebuggerStepThrough]
        public CommandQueryBuilder<TResult> ActAndAssert(Action<DataResult<TResult>, AssertHelper> act)
        {
            var result = Act();
            act(result, _assertHelper);
            return this;
        }

        [DebuggerStepThrough]
        public CommandQueryBuilder<TResult> DbAssert(Action<AssertHelper> assert)
        {
            assert(_assertHelper);
            return this;
        }

        [DebuggerStepThrough]
        public CommandQueryBuilder<TResult> ArrangeSimple(Action<DbOperations> arr)
        {
            _arrangeSimple = arr;
            return this;
        }

        public DataResult<TResult> Act()
        {
            // zum sicher sein
            _assertHelper.InputChecked = false;

            // ReSharper disable once UseNullPropagation
            if (_arrangeSimple != null)
            {
                _arrangeSimple(_dbOperations);
            }

            if (_arrange != null)
            {
                var query = _arrange(_dbOperations);
                _assertHelper.SetParams(query.ParamsDict);
                Debug.WriteLine(query.ToString());
                return Task.Run(() => query.ExecuteAsync(Connection, WrappedTransaction)).GetAwaiter().GetResult();
            }

            return default;
        }

        public void Commit()
        {
            Transaction.Commit();
        }
    }
}