using DeployService;
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
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IBaselineRepository, BaselineRepository>();
builder.Services.AddScoped<IHostRepository, HostRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IScannerRepository, ScannerRepository>();

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
