using System.Net;
using System.Text.Json;

namespace ShoppingCart.Logging
{
    public class ExceptionLogger
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionLogger> _logger;

        public ExceptionLogger(
            RequestDelegate next,
            ILogger<ExceptionLogger> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    message = "An unexpected error occurred. " + ex.Message
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
