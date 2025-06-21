namespace CleanArchitectureProject.WebApi.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log request details
            _logger.LogInformation("Request: {Method} {Path} from {RemoteIpAddress}",
                context.Request.Method,
                context.Request.Path,
                context.Connection.RemoteIpAddress);

            // Log important headers
            if (context.Request.Headers.ContainsKey("User-Agent"))
            {
                _logger.LogInformation("User-Agent: {UserAgent}",
                    context.Request.Headers["User-Agent"]);
            }

            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                _logger.LogInformation("Authorization header present");
            }

            // Add custom response headers
            context.Response.Headers.Add("X-API-Version", "1.0");
            context.Response.Headers.Add("X-Powered-By", "Clean Architecture API");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            await _next(context);

            stopwatch.Stop();

            // Log response details
            _logger.LogInformation("Response: {StatusCode} in {ElapsedMilliseconds}ms",
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}