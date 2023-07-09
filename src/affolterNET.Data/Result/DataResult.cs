using System;
using System.Net;
using Serilog;

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
            Data = default(T)!;
            Exception = ex;
            StatusCode = HttpStatusCode.InternalServerError;
        }

        public DataResult(HttpStatusCode code)
        {
            Data = default(T)!;
            StatusCode = code;
        }

        public T Data { get; }

        public string? SqlCommand { get; set; }

        public Exception? Exception { get; }

        public bool HasException => Exception != null;

        public HttpStatusCode StatusCode { get; }

        public bool IsSuccessful => (int)StatusCode < 399;

        public string GetError()
        {
            if (HasException)
            {
                return Exception!.Message;
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
                logger.Error("Exception in {methodName}", methodName, Exception);
            }
            else
            {
                logger.Error("Error in {methodName}, returning StatusCode {statusCode}", methodName, StatusCode);
            }
        }
    }
}