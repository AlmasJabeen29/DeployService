
using DeployService.Clients;
using DeployService.Services;
using DeployService.Utilities;
using Microsoft.EntityFrameworkCore;
using NumaricConnectt.Infra.Data;
using NumaricConnectt.Infra.Repositories;
using NumarisConnectt.Application.Services.Implementations;
using NumarisConnectt.Application.Services.Interfaces;
using NumarisConnectt.Domain.Interfaces;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);


// Read deploy server IP from command line args, fallback to default if not provided
string deployServerIp = args.Length > 0 ? args[0] : "172.24.70.8";
builder.Services.AddSingleton(deployServerIp);

//configure mySQL database connection
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IBaseLineService, BaseLineService>();
builder.Services.AddScoped<IHostService,HostService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IScannerService, ScannerService>();
builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IBackupService,BackupService>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IBaselineRepository, BaselineRepository>();
builder.Services.AddScoped<IHostRepository, HostRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IScannerRepository, ScannerRepository>();
builder.Services.AddScoped<IAdministratorRepository, AdministratorRepository>();
builder.Services.AddScoped<IBackupRepository, BackupRepository>();
builder.Services.AddScoped<DeploymentService>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7073") }); //Point this uri to your API url.
builder.Services.AddScoped<RequestApiClient>();
builder.Services.AddScoped<RequestStatusUpdater>();
builder.Services.AddScoped<RequestProcessor>();
builder.Services.AddScoped<Mailer>();
builder.Services.AddScoped<BackupApiClient>();
builder.Services.AddSingleton<BackupManager>();


builder.Services.AddHostedService<NumarisWorker>();



Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // Default minimum level
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Error)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Error)
    .WriteTo.Console()
    .WriteTo.File(AppDomain.CurrentDomain.BaseDirectory + "logs/DeployService.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Logging.Services.AddSerilog();
builder.Services.AddWindowsService(options=>
{
    options.ServiceName = "Deploy Service";
});
var host = builder.Build();
host.Run();
