using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener_Shared.Wrappers
{
    public class DataResult<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        // For success
        public static DataResult<T> SuccessResult(T data, string message = "", int statusCode = 200)
        {
            var result = new DataResult<T>();
            result.Success = true;
            result.Data = data;
            result.Message = message;
            result.StatusCode = statusCode;
            return result;
        }

        // For failure
        public static DataResult<T> FailResult(string message, int statusCode = 400)
        {
            var result = new DataResult<T>();
            result.Success = false;
            result.Message = message;
            result.StatusCode = statusCode;
            result.Data = default;
            return result;
        }
    }
}

