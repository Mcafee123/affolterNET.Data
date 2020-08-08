using System;
using System.Net;
using Microsoft.Extensions.Logging;

namespace affolterNET.Data.Result
{
    public class DataResult<T>
    {
        public DataResult(T data, HttpStatusCode code = HttpStatusCode.OK)
        {
            Data = data;
            StatusCode = code;
        }

        public DataResult(Exception ex)
        {
            Exception = ex;
            StatusCode = HttpStatusCode.InternalServerError;
        }

        public DataResult(HttpStatusCode code)
        {
            StatusCode = code;
        }

        public T Data { get; }

        public Exception Exception { get; }

        public bool HasException => Exception != null;

        public HttpStatusCode StatusCode { get; }

        public bool IsSuccessful => (int)StatusCode < 399;

        public string GetError()
        {
            if (HasException)
            {
                return Exception.Message;
            }

            return $"StatusCode: {StatusCode} (no exception thrown";
        }

        public void LogError(ILogger logger, string methodName)
        {
            if (IsSuccessful)
            {
                throw new InvalidOperationException("cannot log error on successful result");
            }

            if (HasException)
            {
                logger.LogError("Exception in {methodName}", methodName, Exception);
            }
            else
            {
                logger.LogError("Error in {methodName}, returning StatusCode {statusCode}", methodName, StatusCode);
            }
        }
    }
}