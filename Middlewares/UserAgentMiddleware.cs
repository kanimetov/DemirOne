namespace Demir.Middlewares;
public class UserAgentMiddleware
{
    private readonly RequestDelegate _next;

    public UserAgentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if the User-Agent header exists
        if (!context.Request.Headers.ContainsKey("User-Agent"))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("User-Agent header is required.");
            return;
        }

        await _next(context);
    }
}
