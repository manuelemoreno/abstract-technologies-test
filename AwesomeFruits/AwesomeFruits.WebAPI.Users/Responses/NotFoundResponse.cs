using System;
using System.Text.Json.Serialization;

namespace AwesomeFruits.WebAPI.Users.Responses;

public class NotFoundResponse
{
    public NotFoundResponse(string statusMessage)
    {
        StatusMessage = statusMessage;
    }

    [JsonPropertyName("status")]
    public int StatusCode { get; set; } = 404;

    [JsonPropertyName("msg")]
    public string StatusMessage { get; set; }

    [JsonPropertyName("date")]
    public DateTime DateOfEvent { get; set; } = DateTime.Now;
}