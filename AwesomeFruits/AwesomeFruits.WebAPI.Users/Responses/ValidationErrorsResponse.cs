using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AwesomeFruits.WebAPI.Users.Responses;

public class ValidationErrorsResponse
{
    public ValidationErrorsResponse()
    {
    }

    public ValidationErrorsResponse(List<string> errors)
    {
        Errors = errors;
    }

    [JsonPropertyName("errors")]
    public List<string> Errors { get; set; }
}