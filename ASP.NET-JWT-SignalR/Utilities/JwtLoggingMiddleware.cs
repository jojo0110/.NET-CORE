namespace LlamaEngineHost.Utilities
{
using Serilog.Context;
using System.Security.Claims;

public class JwtLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public JwtLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var user = context.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            var name = user.FindFirst(ClaimTypes.Name)?.Value;
            var role = user.FindFirst(ClaimTypes.Role)?.Value;


            using (LogContext.PushProperty("UserId", userId))
            using (LogContext.PushProperty("UserEmail", email))
            using (LogContext.PushProperty("UserName", name))
            using (LogContext.PushProperty("UserRole", role))
            {
                await _next(context);
            }
        }
        else
        {
            await _next(context);
        }

        //Console.WriteLine("Authenticated: " + context.User.Identity.IsAuthenticated);
        //Console.WriteLine("Claims: " + string.Join(", ", context.User.Claims.Select(c => $"{c.Type}={c.Value}")));

    }
}

}