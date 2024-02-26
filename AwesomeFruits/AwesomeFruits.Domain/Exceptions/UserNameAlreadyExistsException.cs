using System;

namespace AwesomeFruits.Domain.Exceptions;

public class UserNameAlreadyExistsException : Exception
{
    public UserNameAlreadyExistsException() : base("The username already exists.")
    {
    }

    public UserNameAlreadyExistsException(string message) : base(message)
    {
    }
}