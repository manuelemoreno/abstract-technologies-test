using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AwesomeFruits.Domain.Exceptions;
using AwesomeFruits.WebAPI.Users.Constants;
using AwesomeFruits.WebAPI.Users.Exceptions;
using AwesomeFruits.WebAPI.Users.Responses;
using Microsoft.AspNetCore.Http;

namespace AwesomeFruits.WebAPI.Users.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationErrorsException ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var validationErrorsResponse =
                new ValidationErrorsResponse(ex.Errors.Select(error => error).ToList());

            var validationErrorsSerialized = JsonSerializer.Serialize(validationErrorsResponse);

            response.StatusCode = 400;
            await response.WriteAsync(validationErrorsSerialized);
        }

        catch (UserNameAlreadyExistsException ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var validationErrorsSerialized = JsonSerializer.Serialize(ex.Message);

            response.StatusCode = 400;
            await response.WriteAsync(validationErrorsSerialized);
        }

        catch (UserCredentialsNotValidException ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var validationErrorsSerialized = JsonSerializer.Serialize(ex.Message);

            response.StatusCode = 401;
            await response.WriteAsync(validationErrorsSerialized);
        }

        catch (Exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var validationErrorsSerialized = JsonSerializer.Serialize(ResponseConstants.InternalError);

            response.StatusCode = 500;
            await response.WriteAsync(validationErrorsSerialized);
        }
    }
}