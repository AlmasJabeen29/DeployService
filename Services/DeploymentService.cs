using DeployHandler.AccessClass;
using NumarisConnectt.Application.DataTransferObjects.RetrievalDtos;


namespace DeployService.Services
{
    public class DeploymentService
    {
        private readonly DeployAccess _deployAccess;
        private readonly ILogger<DeploymentService> _logger;

        public DeploymentService(ILogger<DeploymentService> logger, ILogger<DeployAccess> deployAccessLogger, string deployServerIp)
        {
            _logger = logger;
            _deployAccess = new DeployAccess(deployServerIp,deployAccessLogger);
            
        }

        public Task InstallNumarisAsync(ScannerDto scanner, HostDto host, BaseLineDto baseline, BackupDto? backup)
        {
            _logger.LogInformation("Starting Numaris Installation for host: {HostName}", host.HostName);
            try
            {
                _deployAccess.CreateUnattendedDeployment(
                    host.HostName,
                    host.IpAddress,
                    host.MacAddress,
                    baseline.Baseline,
                    scanner.Name,
                    DeployAccess.DeploymentPackageType.MRAWP,
                    true,
                    null!,
                    null!,
                    DeployAccess.SystemVariant.ExFactory,
                    null,
                    backup?.Name! 
                );

                _logger.LogInformation("Completed numaris installation for host: {HostName}", host.HostName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during numaris installation for host: {HostName}", host.HostName);
                throw;
            }
            return Task.CompletedTask;
        }
    }

}
