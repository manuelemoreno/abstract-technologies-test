using System;

namespace AwesomeFruits.Domain.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException() : base("The requested entity was not found.")
    {
    }

    public EntityNotFoundException(string message) : base(message)
    {
    }
}