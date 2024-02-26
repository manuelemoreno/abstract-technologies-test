using System.Collections.Generic;
using AwesomeFruits.Application.DTOs;
using AwesomeFruits.WebAPI.Constants;

namespace AwesomeFruits.WebAPI.Validations;

public class SaveFruitDtoValidator
{
    public List<string> ValidateSaveFruitDto(SaveFruitDto updateFruitDto)
    {
        var validationErrors = new List<string>();

        if (string.IsNullOrEmpty(updateFruitDto.Name))
            validationErrors.Add(ValidationConstants.NameIsRequired);

        if (string.IsNullOrEmpty(updateFruitDto.Description))
            validationErrors.Add(ValidationConstants.DescriptionIsRequired);

        return validationErrors;
    }
}