
using System.Runtime.Versioning;
using DeployService.Services;
using DeployService.Utilities;
using NumarisConnectt.Application.DataTransferObjects.RetrievalDtos;

public class NumarisWorker : BackgroundService
{
    private readonly ILogger<NumarisWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public NumarisWorker(
        ILogger<NumarisWorker> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    [SupportedOSPlatform("windows")]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Checking for new requests at: {time}", DateTime.Now);

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

    [SupportedOSPlatform("windows")]
    private async Task ProcessRequestAsync(RequestDto request, IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var requestProcessor = scope.ServiceProvider.GetRequiredService<RequestProcessor>();
            var deploymentService = scope.ServiceProvider.GetRequiredService<DeploymentService>();
            var requestStatusUpdater = scope.ServiceProvider.GetRequiredService<RequestStatusUpdater>();
            var mailer = scope.ServiceProvider.GetRequiredService<Mailer>();
            await mailer.SendEmailOnRequestSubmission(request);

            await Task.Delay(1000); // Simulate some processing delay
            var (user, host, scanner, baseline) = await requestProcessor.RetrieveRequestDataAsync(request);

            if (user != null && host != null && scanner != null && baseline != null)
            {
                await requestStatusUpdater.UpdateRequestStatusAsync(request, Status.OnGoing);
                await Task.Delay(3000); // Simulate some processing delay
                //await deploymentService.InstallNumarisAsync(scanner, host, baseline);
                await requestStatusUpdater.UpdateRequestStatusAsync(request,Status.Completed);
                await mailer.SendEmailOnSuccessfulInstallation(request);
            }
            else
            {
                _logger.LogError("Failed to retrieve required data for request: {0}", request.Id);
            }
        }
    }
}



