using Dacoder.Webfileserver;
using Dacoder.Webfileserver.ActionFilters;
using Dacoder.Webfileserver.Middleware;
using Dacoder.Webfileserver.Settings;
using Microsoft.Extensions.FileProviders;
using Serilog;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
var settings = builder.Configuration.Get<AppSettings>();
builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddSingleton(settings); //Add a copy off the application configuration into IoC.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped(sp => new PoorManClientCredentialFilter(settings.PoorManClientCredential, builder.Environment));
builder.WebHost.ConfigureKestrel((context, serverOptions) =>
{
    serverOptions.Listen(IPAddress.Any, settings.ApplicationPort, listenOptions =>
    {
        listenOptions.UseHttps(settings.RelativePfxCertificateFileName, settings.CertificatePassword);
    });
});

//builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
builder.WebHost.UseUrls($"https://*:{settings.ApplicationPort}");
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<DownloadHistoryLoggingMiddleware>();
app.UseMiddleware<BandwidthThrottlingMiddleware>(400000); //~400 Kilabytes/Second
app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(settings.FileServerPath),
    RequestPath = new PathString(settings.WebsiteAlias),
    EnableDirectoryBrowsing = true,
});
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(settings.FileServerPath),
    RequestPath = new PathString(settings.WebsiteAlias),
    ServeUnknownFileTypes = true,
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
