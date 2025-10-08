using DeployHandler.AccessClass;
using DeployService.Clients;
using DeployService.Services;
using DeployService.Utilities;
using NumarisConnectt.Application.DataTransferObjects.RetrievalDtos;
using System.Runtime.Versioning;


public class NumarisWorker : BackgroundService
{
    private readonly ILogger<NumarisWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly BackupManager _backupManager;

    public NumarisWorker(
        ILogger<NumarisWorker> logger,
        IServiceProvider serviceProvider, BackupManager backupManager)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _backupManager = backupManager;
    }

    [SupportedOSPlatform("windows")]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("NumarisWorker ExecuteAsync started at: {time}", DateTime.Now);
        await _backupManager.LoadAllBackupsAsync();
        try
        {
            _logger.LogInformation("Checking for new requests at: {time}", DateTime.Now);
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var requestProcessor = scope.ServiceProvider.GetRequiredService<RequestProcessor>();
                    var newRequests = await requestProcessor.GetNewRequestsAsync();

                    foreach (var request in newRequests)
                    {
                        await ProcessRequestAsync(request, scope.ServiceProvider);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in ExecuteAsync.");
        }
        _logger.LogInformation("NumarisWorker ExecuteAsync stopped at: {time}", DateTime.Now);
    }

    [SupportedOSPlatform("windows")]
    private async Task ProcessRequestAsync(RequestDto request, IServiceProvider serviceProvider)
    {
        _logger.LogInformation("Processing request {0}", request.Id);
        try
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var requestProcessor = scope.ServiceProvider.GetRequiredService<RequestProcessor>();
                var deploymentService = scope.ServiceProvider.GetRequiredService<DeploymentService>();
                var requestStatusUpdater = scope.ServiceProvider.GetRequiredService<RequestStatusUpdater>();
                var mailer = scope.ServiceProvider.GetRequiredService<Mailer>();
                await mailer.SendEmailOnRequestSubmission(request);

                await Task.Delay(1000); // Simulate some processing delay
                var (user, host, scanner, baseline, assignee) = await requestProcessor.RetrieveRequestDataAsync(request);

                if (user != null && host != null && scanner != null && baseline != null)
                {
                    await requestStatusUpdater.UpdateRequestStatusAsync(request, Status.OnGoing);
                    await Task.Delay(3000); // Simulate some processing delay
                    try
                    {
                        BackupDto? backup = _backupManager.GetBackup(baseline.Baseline, scanner.Name);
                        await deploymentService.InstallNumarisAsync(scanner, host, baseline, backup);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error : ", ex);
                        await requestStatusUpdater.UpdateRequestStatusAsync(request, Status.Failed);
                        throw;
                    }
                    
                    await requestStatusUpdater.UpdateRequestStatusAsync(request, Status.Completed);
                    await mailer.SendEmailOnSuccessfulInstallation(request);
                }
                else
                {
                    _logger.LogError("Failed to retrieve required data for request: {0}", request.Id);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while processing request {RequestId}", request.Id);
        }
    }

}



