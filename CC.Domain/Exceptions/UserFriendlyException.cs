namespace CC.Domain.Exceptions
{
    public class UserFriendlyException : Exception
    {
        public int StatusCode { get; }

        public UserFriendlyException(string message, int statusCode = 400)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
