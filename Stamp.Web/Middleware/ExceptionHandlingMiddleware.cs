using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Stamp.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace Stamp.Web.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware( RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger )
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync( HttpContext context )
        {
            try
            {
                await _next( context );
            }
            catch( InvalidRoleAssignmentException ex )
            {
                _logger.LogWarning( ex, "Forbidden role assignment attempt." );
                await WriteErrorResponseAsync( context, HttpStatusCode.Forbidden, ex.Message );
            }
            catch( Exception ex )
            {
                _logger.LogError( ex, "Unhandled exception occurred." );
                await WriteErrorResponseAsync( context, HttpStatusCode.InternalServerError,
                    "An unexpected error occurred. Please contact support." );
            }
        }

        private static Task WriteErrorResponseAsync( HttpContext context, HttpStatusCode statusCode, string message )
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ( int )statusCode;

            var errorResponse = new
            {
                status = statusCode,
                error = message,
                traceId = context.TraceIdentifier
            };

            var json = JsonSerializer.Serialize( errorResponse );
            return context.Response.WriteAsync( json );
        }
    }
}
