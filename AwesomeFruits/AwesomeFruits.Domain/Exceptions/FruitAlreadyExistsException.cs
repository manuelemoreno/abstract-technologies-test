using System;

namespace AwesomeFruits.Domain.Exceptions;

public class FruitAlreadyExistsException : Exception
{
    public FruitAlreadyExistsException() : base("There is a fruit with that name already")
    {
    }

    public FruitAlreadyExistsException(string message) : base(message)
    {
    }
}