using System.Data;

namespace affolterNET.Data.TestHelpers.Interfaces
{
    public interface ITransactionDecorator : IDbTransaction
    {
        IDbTransaction WrappedTransaction { get; }
    }
}