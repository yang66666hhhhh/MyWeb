namespace WebApplication1.Common;

public class ApiResult
{
    public int Code { get; set; }

    public string Message { get; set; } = "success";

    public static ApiResult Success(string message = "success")
    {
        return new ApiResult { Code = 0, Message = message };
    }

    public static ApiResult Fail(string message, int code = 1)
    {
        return new ApiResult { Code = code, Message = message };
    }
}

public class ApiResult<T> : ApiResult
{
    public T? Data { get; set; }

    public static ApiResult<T> Success(T data, string message = "success")
    {
        return new ApiResult<T> { Code = 0, Message = message, Data = data };
    }

    public new static ApiResult<T> Fail(string message, int code = 1)
    {
        return new ApiResult<T> { Code = code, Message = message };
    }
}
