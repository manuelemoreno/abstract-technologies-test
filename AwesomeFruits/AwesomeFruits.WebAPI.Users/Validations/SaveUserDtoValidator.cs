using System.Collections.Generic;
using AwesomeFruits.Application.DTOs;
using AwesomeFruits.WebAPI.Users.Constants;

namespace AwesomeFruits.WebAPI.Users.Validations;

public class SaveUserDtoValidator
{
    public List<string> ValidateSaveUserDto(SaveUserDto saveUserDto)
    {
        var validationErrors = new List<string>();

        if (string.IsNullOrEmpty(saveUserDto.FirstName))
            validationErrors.Add(ValidationConstants.FirstNameIsRequired);

        if (string.IsNullOrEmpty(saveUserDto.LastName))
            validationErrors.Add(ValidationConstants.LastNameIsRequired);

        if (string.IsNullOrEmpty(saveUserDto.UserName))
            validationErrors.Add(ValidationConstants.UserNameIsRequired);

        if (string.IsNullOrEmpty(saveUserDto.Password))
            validationErrors.Add(ValidationConstants.PasswordIsRequired);

        return validationErrors;
    }
}