using Serilog;

namespace FoodSafetyTracker.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var userName = context.User?.Identity?.Name ?? "Anonymous";
                Log.Error(ex,
                    "Unhandled exception. User={UserName} Path={Path} Method={Method}",
                    userName,
                    context.Request.Path,
                    context.Request.Method);

                context.Response.Redirect("/Home/Error");
            }
        }
    }
}