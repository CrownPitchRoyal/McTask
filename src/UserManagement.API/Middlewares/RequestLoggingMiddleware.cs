using System.Text;
using Serilog;

namespace UserManagement.API.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        context.Request.EnableBuffering();
        
        var request = context.Request;
        var body = string.Empty;
        
        // Read request 
        if (request.ContentLength is > 0 and < 1024 * 1024) // Max 1MB
        {
            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
        }
        
        Log.Information("Incoming request: {Method} {Path}{QueryString} - Body: {Body}",
            request.Method, request.Path, request.QueryString, body);
        
        await next(context);
    }
}