namespace PasswordManager.Application.Core
{
    public class ApiResult<T>
    {
        public bool IsSuccess { get; set; }
        public bool IsForbidden { get; set; }
        public T? Value { get; set; }
        public string? Error { get; set; }

        public static ApiResult<T> Success(T value) => new ApiResult<T>
            { IsSuccess = true, IsForbidden = false, Value = value };

        public static ApiResult<T> Failure(string error) => new ApiResult<T>
            { IsSuccess = false, IsForbidden = false, Error = error };

        public static ApiResult<T> Forbidden() => new ApiResult<T> { IsForbidden = true, IsSuccess = false };
    }
}