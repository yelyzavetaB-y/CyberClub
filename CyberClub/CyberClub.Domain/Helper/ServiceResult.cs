using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Helper
{
    public class ServiceResult
    {
        public bool IsSuccess { get; protected set; }
        public string? ErrorMessage { get; protected set; }

        public static ServiceResult Success() =>
            new ServiceResult { IsSuccess = true };

        public static ServiceResult Failure(string errorMessage) =>
            new ServiceResult { IsSuccess = false, ErrorMessage = errorMessage };
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; private set; }

        public static ServiceResult<T> Success(T data) =>
            new ServiceResult<T> { IsSuccess = true, Data = data };

        public static ServiceResult<T> Failure(string errorMessage) =>
            new ServiceResult<T> { IsSuccess = false, ErrorMessage = errorMessage };
        public static ServiceResult<T> Failure(string errorMessage, T data) =>
            new ServiceResult<T> { IsSuccess = false, ErrorMessage = errorMessage, Data = data };
    }
}