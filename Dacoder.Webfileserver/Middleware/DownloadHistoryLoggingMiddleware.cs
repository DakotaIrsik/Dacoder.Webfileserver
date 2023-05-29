using Dacoder.Webfileserver.Settings;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using System.Web;

namespace Dacoder.Webfileserver;

public class DownloadHistoryLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AppSettings _settings;

    public DownloadHistoryLoggingMiddleware(RequestDelegate next, IOptions<AppSettings> options)
    {

        _next = next;
        _settings = options.Value;
        Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, _settings.DownloadHistoryFolderName));
    }

    public async Task Invoke(HttpContext context)
    {            
        var regex = new Regex(@"\.[a-zA-Z0-9]{2,3}$");
        var path = context.Request.Path.ToString();
        var fileLink = regex.IsMatch(path);
        var filesToExclude = _settings.DownloadHistoryExclusionCsvFileList.Split(",");
        var shouldExclude = filesToExclude.Any(de => Path.GetFileNameWithoutExtension(path).Contains(de));

        if (fileLink && !shouldExclude)
        {
            var fileName = Path.GetFileName(path);
            fileName = Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty)); // Sanitize filename
            await File.AppendAllLinesAsync(Path.Combine(AppContext.BaseDirectory, _settings.DownloadHistoryFolderName, $"{HttpUtility.UrlDecode(fileName)}.log"), new List<string>()
            {
                $"{context.Connection.RemoteIpAddress}  {DateTime.Now.ToString(_settings.TimeStructure)}"
            });
        }

        await _next(context);
    }
}

