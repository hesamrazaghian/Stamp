using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Stamp.Application.DTOs;
using Stamp.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace Stamp.Web.Middleware
{
    /// <summary>
    /// Middleware for handling global exceptions and formatting responses.
    /// </summary>
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
            catch( ValidationException validationEx ) // Handling FluentValidation errors
            {
                _logger.LogWarning( validationEx, "Validation failed." );
                var errors = validationEx.Errors.Select( e => e.ErrorMessage ).ToList( );
                await WriteErrorResponseAsync( context, HttpStatusCode.BadRequest, "Validation failed", errors );
            }
            catch( KeyNotFoundException ex )
            {
                _logger.LogWarning( ex, "Resource not found." );
                await WriteErrorResponseAsync( context, HttpStatusCode.NotFound, ex.Message );
            }
            catch( Exception ex )
            {
                _logger.LogError( ex, "Unhandled exception occurred." );
                await WriteErrorResponseAsync( context, HttpStatusCode.InternalServerError,
                    "An unexpected error occurred. Please contact support." );
            }
        }

        private static Task WriteErrorResponseAsync( HttpContext context, HttpStatusCode statusCode, string message, List<string>? errors = null )
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ( int )statusCode;

            var errorResponse = new ErrorResponse
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>( ),
                TraceId = context.TraceIdentifier
            };

            var json = JsonSerializer.Serialize( errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            } );

            return context.Response.WriteAsync( json );
        }
    }
}
