namespace EmailService.Exceptions;

public class EmailServiceException : Exception
{
    public EmailServiceException()
    {
    }

    public EmailServiceException(string message)
        : base(message)
    {
    }

    public EmailServiceException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
