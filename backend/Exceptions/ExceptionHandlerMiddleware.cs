using System.Text.Json;

namespace backend.Exceptions
{
    public class DomainException : Exception { public DomainException(string msg) : base(msg) { } }
    public class StateException : Exception { public StateException(string msg) : base(msg) { } }
    public class ExternalServiceException : Exception { public ExternalServiceException(string msg) : base(msg) { } }

    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
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
                await httpContext.Response.WriteAsJsonAsync(new { error = ex.Message }, jsonSerializerOptions);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Invalid domain");
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(new { error = ex.Message }, jsonSerializerOptions);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogWarning(ex, "External service failed");
                httpContext.Response.StatusCode = StatusCodes.Status502BadGateway;
                await httpContext.Response.WriteAsJsonAsync(new { error = ex.Message }, jsonSerializerOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unhandled exception");
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                await httpContext.Response.WriteAsJsonAsync(new { error = ex.Message }, jsonSerializerOptions);
            }
        }
    }

}