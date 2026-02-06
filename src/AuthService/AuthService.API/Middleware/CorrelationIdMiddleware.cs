namespace AuthService.API.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var correlationId = context.Request.Headers["X-Correlation-Id"].ToString();

            using(_logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId
            }))
            {
                _logger.LogInformation("Request Started");
                await _next(context);
                _logger.LogInformation("Request Finished");
            }
        }
    }
}
