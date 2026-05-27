namespace PremierLeague.Application.Common.Models;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public string? Error { get; private set; }
    public ResultErrorType ErrorType { get; private set; }

    private Result() { }

    public static Result<T> Success(T value)
        => new() { IsSuccess = true, Value = value };

    public static Result<T> Failure(string error, ResultErrorType errorType = ResultErrorType.General)
        => new() { IsSuccess = false, Error = error, ErrorType = errorType };

    public static Result<T> NotFound(string error)
        => Failure(error, ResultErrorType.NotFound);

    public static Result<T> ValidationFailure(string error)
        => Failure(error, ResultErrorType.Validation);
}

public enum ResultErrorType
{
    General,
    NotFound,
    Validation,
    Conflict,
    Unauthorized
}
