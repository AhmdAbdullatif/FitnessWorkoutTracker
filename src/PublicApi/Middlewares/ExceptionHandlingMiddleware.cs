using Application.Exceptions;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace PublicApi.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (DomainException ex)
        {
            await WriteProperProblemDetails(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (InvalidUserCredentialsException ex)
        {
            await WriteProperProblemDetails(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (NotFoundException ex)
        {
            await WriteProperProblemDetails(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (EmailConflictException ex)
        {
            await WriteProperProblemDetails(context, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unhandled exception at {Path}. TraceId: {TraceId}",
                context.Request.Path,
                context.TraceIdentifier
            );

            await WriteProperProblemDetails(context,
                StatusCodes.Status500InternalServerError,
                "Something went wrong. try again later!"
            );
        }
    }

    public static async Task WriteProperProblemDetails(HttpContext context, int statusCode, string detail)
    {
        var problem = new ProblemDetails()
        {
            Status = statusCode,
            Title = ReasonPhrases.GetReasonPhrase(statusCode),
            Detail = detail,
            Instance = context.Request.Path,
            Type = $"https://httpstatuses.com/{statusCode}"
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            problem.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    }
}
