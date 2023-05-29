using Dacoder.Webfileserver.ActionFilters;
using Dacoder.Webfileserver.Settings;
using Dacoder.Webfileserver.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.NetworkInformation;

namespace Dacoder.Webfileserver.Controllers;

[ApiController]
[Route("[controller]")]
[ServiceFilter(typeof(PoorManClientCredentialFilter))]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly AppSettings _settings;

    public UserController(ILogger<UserController> logger, IOptions<AppSettings> options)
    {
        _logger = logger;
        _settings = options.Value;
    }

    [HttpGet]
    public ActionResult<List<User>> Get()
    {
        var properties = IPGlobalProperties.GetIPGlobalProperties();
        var httpConnections = (from connection in properties.GetActiveTcpConnections()
                               where connection.LocalEndPoint.Port == _settings.ApplicationPort
                               select connection);

        List<User> users = new List<User>();

        foreach (var connection in httpConnections)
        {
            users.Add(new User()
            {
                RemoteEndPoint = connection.RemoteEndPoint.ToString(),
                State = connection.State.ToString()
            });
        }

        return Ok(users);
    }
}