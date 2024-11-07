namespace MeuPatrimonio.User.Models;

public class Result
{
    private Result(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public bool IsSuccess { get; }
    public string Message { get; }

    public static Result Success(string message)
    {
        return new Result(true, message);
    }

    public static Result Failure(string message)
    {
        return new Result(false, message);
    }
}