namespace PromoCodeFactory.Core.Domain
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

        public static Result Success()
        {
            return new Result { IsSuccess = true };
        }

        public static Result Failure(string errorMessage)
        {
            return new Result { IsSuccess = false, ErrorMessage = errorMessage };
        }
    }

    public class Result<T> : Result
    {
        public T Value { get; set; }
    }
}
