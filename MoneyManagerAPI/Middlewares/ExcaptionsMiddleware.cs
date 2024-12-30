using System.Net;

namespace MoneyManagerAPI.Middlewares
{
    public class ExceptionsMiddleware(ILogger<ExceptionsMiddleware> logger, RequestDelegate next)
    {
        private readonly ILogger<ExceptionsMiddleware> _logger = logger;
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"There was an exception in the middleware: {ex.Message}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var result = exception.Message;

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(result);
        }
    }
}