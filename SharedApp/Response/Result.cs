/// <summary>
/// Represents the result of an operation, which can be either successful or failed.
/// </summary>
/// <typeparam name="T">The type of the result value.</typeparam>
public class Result<T>
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// The result value if the operation was successful.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// The error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Private constructor to enforce the use of factory methods.
    /// </summary>
    private Result(bool isSuccess, T value, string? errorMessage)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a successful result with a value.
    /// </summary>
    /// <param name="value">The result value.</param>
    /// <returns>A successful <see cref="Result{T}"/> object.</returns>
    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, default);
    }

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failed <see cref="Result{T}"/> object.</returns>
    public static Result<T> Failure(string errorMessage)
    {
        return new Result<T>(false, default, errorMessage);
    }
}
