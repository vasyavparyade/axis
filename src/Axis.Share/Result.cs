using System;
using System.Threading.Tasks;

namespace Axis.Share
{
    public class Result
    {
        public Result()
        {
        }

        public Result(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(error));

            Error = error;
        }

        public bool IsSuccessful => string.IsNullOrWhiteSpace(Error);

        public string Error { get; private set; }

        public static implicit operator bool(Result result) => result?.IsSuccessful ?? false;

        public static Result Successful => new Result();

        public static Result Fail(string error) => new Result(error);

        public static Result From(Action action)
        {
            try
            {
                action();

                return Successful;
            }
            catch (Exception exception)
            {
                return Fail(exception.Message);
            }
        }

        public static async Task<Result> From(Func<Task> func)
        {
            try
            {
                await func().ConfigureAwait(false);

                return Successful;
            }
            catch (Exception exception)
            {
                return Fail(exception.Message);
            }
        }
    }

    public class Result<T> : Result
    {
        public Result(T result)
        {
            Value = result;
        }

        public Result(string error) : base(error)
        {
        }

        public T Value { get; private set; }

        public static implicit operator bool(Result<T> result) => result?.IsSuccessful ?? false;

        public new static Result<T> Fail(string error) => new Result<T>(error);

        public static Result<T> From(T value) => new Result<T>(value);

        public static Result<T> From(Func<T> func)
        {
            try
            {
                var result = func();

                return From(result);
            }
            catch (Exception exception)
            {
                return Fail(exception.Message);
            }
        }

        public static async Task<Result<T>> From(Func<Task<T>> func)
        {
            try
            {
                var result = await func().ConfigureAwait(false);

                return From(result);
            }
            catch (Exception exception)
            {
                return Fail(exception.Message);
            }
        }
    }
}
