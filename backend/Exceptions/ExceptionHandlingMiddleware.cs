using System.Text.Json;

namespace backend.Exceptions
{
    public class DomainException : Exception { public DomainException(string msg) : base(msg) { } }
    public class StateException : Exception { public StateException(string msg) : base(msg) { } }
    public class InvalidVerifierException : Exception { public InvalidVerifierException(string msg) : base(msg) { } }
    public class ExternalServiceException : Exception { public ExternalServiceException(string msg) : base(msg) { } }

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (StateException ex)
            {
                _logger.LogWarning(ex, "Invalid state");
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Invalid domain");
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogWarning(ex, "External service failed");
                httpContext.Response.StatusCode = StatusCodes.Status502BadGateway;
                await httpContext.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            catch (InvalidVerifierException ex)
            {
                _logger.LogWarning(ex, "Invalid verifier");
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            catch (HttpRequestException ex)
            { 
                _logger.LogWarning(ex, "HTTP request failed");
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
             catch (JsonException ex)
            { 
                _logger.LogWarning(ex, "Json failed");
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unhandled exception");
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await httpContext.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            
        }
    }

}