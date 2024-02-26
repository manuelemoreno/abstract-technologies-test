using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AwesomeFruits.WebAPI.Responses;

public class ErrorListResponse
{
    public ErrorListResponse()
    {
    }

    public ErrorListResponse(List<string> errors)
    {
        Errors = errors;
    }

    [JsonPropertyName("errors")]
    public List<string> Errors { get; set; }
}