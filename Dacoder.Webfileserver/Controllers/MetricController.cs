using Dacoder.Webfileserver.Settings;
using Dacoder.Webfileserver.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Dacoder.Webfileserver.Controllers;

[ApiController]
[Route("[controller]")]
public class MetricController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly AppSettings _settings;

    public MetricController(ILogger<UserController> logger, IOptions<AppSettings> options)
    {
        _logger = logger;
        _settings = options.Value;
    }

    [HttpGet("TopDownloads")]
    public ActionResult<List<User>> TopDownloads(int x)
    {
        var downloads = Directory.GetFiles(AppContext.BaseDirectory + _settings.DownloadHistoryFolderName);

        return Ok(downloads.OrderByDescending(d => System.IO.File.ReadAllLines(d).Count()).Select(d => $"{Path.GetFileNameWithoutExtension(d)} - {System.IO.File.ReadAllLines(d).Count()}").Take(x));
    }
}