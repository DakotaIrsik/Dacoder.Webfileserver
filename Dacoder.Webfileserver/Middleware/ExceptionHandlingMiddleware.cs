using Dacoder.Webfileserver.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Dacoder.Webfileserver;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AppSettings _settings;

    public ExceptionHandlingMiddleware(RequestDelegate next, IOptions<AppSettings> options)
    {
        _next = next;
        _settings = options.Value;
    }

    public async Task Invoke(HttpContext context, IWebHostEnvironment env, ILogger<ExceptionHandlingMiddleware> logger)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, env, logger);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, IWebHostEnvironment env, ILogger logger)
    {

        await File.AppendAllLinesAsync($"{AppContext.BaseDirectory}{_settings.ApplicationName}.error-{DateTime.Now.ToString(_settings.TimeStructure)}.log", new List<string>()
       {
           $"{DateTime.Now.ToString(_settings.TimeStructure)}",
           $"{exception.Message},",
           $"{exception.StackTrace}",
           string.Empty,
           $"{exception.InnerException?.Message}",
           $"{exception.InnerException?.StackTrace}",
       });


        string result = JsonConvert.SerializeObject(new
        {
            Error = exception.Message,
            exception.StackTrace,
            Name = "Unhandled exception: " + exception.GetType().Name,
            exception?.InnerException
        });


        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync(result);
        await context.Response.CompleteAsync();
    }
}

