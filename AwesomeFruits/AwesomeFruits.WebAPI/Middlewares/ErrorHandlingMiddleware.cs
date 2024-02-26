using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AwesomeFruits.Domain.Exceptions;
using AwesomeFruits.WebAPI.Constants;
using AwesomeFruits.WebAPI.Exceptions;
using AwesomeFruits.WebAPI.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AwesomeFruits.WebAPI.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogError($"EntityNotFoundException: {ex.StackTrace}");

            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse(ResponseConstants.FruitNotFound);

            var serializeErrorResponse = JsonSerializer.Serialize(errorResponse);

            response.StatusCode = 404;
            await response.WriteAsync(serializeErrorResponse);
        }
        catch (ValidationErrorsException ex)
        {
            _logger.LogError($"ValidationErrorsException: {ex.StackTrace}");

            var response = context.Response;
            response.ContentType = "application/json";

            var validationErrorsResponse =
                new ErrorListResponse(ex.Errors.Select(error => error).ToList());

            var validationErrorsSerialized = JsonSerializer.Serialize(validationErrorsResponse);

            response.StatusCode = 400;
            await response.WriteAsync(validationErrorsSerialized);
        }

        catch (FruitAlreadyExistsException ex)
        {
            _logger.LogError($"FruitAlreadyExistsException: {ex.StackTrace}");

            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse(ex.Message);

            var serializeErrorResponse = JsonSerializer.Serialize(errorResponse);

            response.StatusCode = 400;
            await response.WriteAsync(serializeErrorResponse);
        }

        catch (Exception ex)
        {
            _logger.LogError($"Unknown Exception: {ex.StackTrace}");

            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse(ResponseConstants.InternalError);

            var serializeErrorResponse = JsonSerializer.Serialize(errorResponse);

            response.StatusCode = 500;
            await response.WriteAsync(serializeErrorResponse);
        }
    }
}