using System;

namespace AwesomeFruits.Domain.Exceptions;

public class UserCredentialsNotValidException : Exception
{
    public UserCredentialsNotValidException() : base("The user is not valid. Verify credentials.")
    {
    }

    public UserCredentialsNotValidException(string message) : base(message)
    {
    }
}