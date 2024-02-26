using System.Text.Json.Serialization;

namespace AwesomeFruits.WebAPI.Responses;

public class ErrorResponse
{
    public ErrorResponse(string message)
    {
        Message = message;
    }

    [JsonPropertyName("msg")]
    public string Message { get; set; }
}