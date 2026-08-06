namespace HotelBookingSystem.Application.Common;

public enum ResultType
{
    Success,
    BadRequest,
    NotFound,
    Conflict,
    Unauthorized
}

public class Result<T>
{
    public bool Success { get; private set; }
    public string? Error { get; private set; }
    public T? Data { get; private set; }
    public ResultType Type { get; private set; } = ResultType.Success;

    public static Result<T> Ok(T data) => new() { Success = true, Data = data, Type = ResultType.Success };
    public static Result<T> Fail(string error, ResultType type = ResultType.BadRequest) => new() { Success = false, Error = error, Type = type };
    public static Result<T> NotFound(string error) => new() { Success = false, Error = error, Type = ResultType.NotFound };
    public static Result<T> Conflict(string error) => new() { Success = false, Error = error, Type = ResultType.Conflict };
    public static Result<T> BadRequest(string error) => new() { Success = false, Error = error, Type = ResultType.BadRequest };
    public static Result<T> Unauthorized(string error) => new() { Success = false, Error = error, Type = ResultType.Unauthorized };
}

