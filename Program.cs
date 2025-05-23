
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
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IBaselineRepository, BaselineRepository>();
builder.Services.AddScoped<IHostRepository, HostRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IScannerRepository, ScannerRepository>();
builder.Services.AddScoped<IAdministratorRepository, AdministratorRepository>();
builder.Services.AddScoped<DeploymentService>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7073") }); //Point this uri to your API url.
builder.Services.AddScoped<ApiClient>();
builder.Services.AddScoped<RequestStatusUpdater>();
builder.Services.AddScoped<RequestProcessor>();
builder.Services.AddScoped<Mailer>();

builder.Services.AddHostedService<NumarisWorker>();



Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().
    WriteTo.Console().WriteTo
    .File(AppDomain.CurrentDomain.BaseDirectory+"logs/DeployService.txt", rollingInterval: RollingInterval.Day).CreateLogger();
builder.Logging.Services.AddSerilog();
builder.Services.AddWindowsService(options=>
{
    options.ServiceName = "Deploy Service";
});
var host = builder.Build();
host.Run();
