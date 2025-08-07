using DeployHandler.AccessClass;
using NumarisConnectt.Application.DataTransferObjects.RetrievalDtos;


namespace DeployService.Services
{
    public class DeploymentService
    {
        private readonly DeployAccess _deployAccess;

        public DeploymentService()
        {
            _deployAccess = new DeployAccess("172.24.70.8");
        }

        public Task InstallNumarisAsync(ScannerDto scanner, HostDto host, BaseLineDto baseline)
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
                DeployAccess.SystemVariant.ExFactory
            );

            return Task.CompletedTask;
        }
    }

}
