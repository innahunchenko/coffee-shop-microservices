namespace Foundation.Exceptions
{
    public class CustomRedisException : Exception
    {
        public CustomRedisException (string message) : base (message) { }

        public CustomRedisException(string message, string details) : base(message) 
        { 
            Details = details;
        }

        public string? Details{ get; }
    }
}
