using System;
using System.Collections.Generic;

namespace AwesomeFruits.WebAPI.Users.Exceptions;

public class ValidationErrorsException : Exception
{
    public ValidationErrorsException(List<string> errors)
        : base("Validation failed")
    {
        Errors = errors;
    }

    public List<string> Errors { get; }
}