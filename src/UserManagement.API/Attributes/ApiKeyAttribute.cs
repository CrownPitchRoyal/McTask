using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using UserManagement.Persistence.Data;

namespace UserManagement.API.Attributes;

public class ApiKeyAttribute : Attribute, IAsyncActionFilter
{
    private const string ApiKeyHeaderName = "apikey";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var foundApiKey))
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                StatusCode = 401,
                Message = "API Key is missing."
            });
            return;
        }

        var dbContext = context.HttpContext.RequestServices.GetRequiredService<UserDbContext>();

        var apiKey = await dbContext.ApiKeys.FirstOrDefaultAsync(a => a.Key == Guid.Parse(foundApiKey));
        if (apiKey == null)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                StatusCode = 401,
                Message = "API Key is invalid."
            });
            return;
        }

        if (DateTime.UtcNow > apiKey.DateCreated.AddMinutes(10)) // Test pru
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                StatusCode = 401,
                Message = "API Key is expired."
            });
            return;
        }

        await next();
    }
}