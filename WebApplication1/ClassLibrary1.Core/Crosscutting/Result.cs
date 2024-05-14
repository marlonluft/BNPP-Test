namespace ClassLibrary1.Core.Crosscutting
{
    public abstract class ResultAbstract
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public class Result : ResultAbstract
    {
        private Result()
        {
            Success = true;
        }

        private Result(string errorMessage)
        {
            Message = errorMessage;
            Success = false;
        }

        public static Result FromSucess() => new();
        public static Result FromError(string errorMessage) => new(errorMessage);
    }

    public class Result<T> : ResultAbstract
    {
        private Result(T data)
        {
            Success = true;
            Data = data;
        }

        private Result(string errorMessage)
        {
            Message = errorMessage;
            Success = false;
        }

        public T Data { get; set; }

        public static Result<T> FromSucess(T data) => new(data);
        public static Result<T> FromError(string errorMessage) => new(errorMessage);
    }
}
