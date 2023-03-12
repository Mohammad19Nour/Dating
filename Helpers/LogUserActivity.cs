using Microsoft.AspNetCore.Mvc.Filters;

namespace Dating_App.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        return null;
    }
}