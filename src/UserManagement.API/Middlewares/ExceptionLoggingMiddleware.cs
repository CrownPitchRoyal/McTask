using System.Text.Json;
using Serilog;

namespace UserManagement.API.Middlewares;

public class ExceptionLoggingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            const string err = "An unhandled exception occurred.";

            Log.Error(e, err);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;

            var response = new
            {
                StatusCode = 500,
                Message = err,
                Detail = e.Message,
                StackTrace = e.StackTrace
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}