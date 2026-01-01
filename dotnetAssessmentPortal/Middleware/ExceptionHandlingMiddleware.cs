using dotnetAssessmentPortal.Exceptions;
using dotnetAssessmentPortal.Models.DTOs;
using System.Net;
using System.Text.Json;

namespace dotnetAssessmentPortal.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var errorResponse = new ErrorResponseDto
            {
                Timestamp = DateTime.UtcNow
            };

            switch (exception)
            {
                case NotFoundException notFoundException:
                    errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Message = notFoundException.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    _logger.LogWarning(exception, "Resource not found: {Message}", notFoundException.Message);
                    break;

                case BusinessException businessException:
                    errorResponse.StatusCode = businessException.StatusCode;
                    errorResponse.Message = businessException.Message;
                    context.Response.StatusCode = businessException.StatusCode;
                    _logger.LogWarning(exception, "Business exception occurred: {Message}", businessException.Message);
                    break;

                default:
                    errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = "An error occurred while processing your request";
                    errorResponse.Details = exception.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
                    break;
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonResponse = JsonSerializer.Serialize(errorResponse, jsonOptions);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}

