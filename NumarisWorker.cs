
using DeployHandler.AccessClass;
using Microsoft.AspNetCore.Mvc;
using NumarisConnectt.Api.Controllers;
using NumarisConnectt.Application.DataTransferObjects.CreationDtos;
using NumarisConnectt.Application.DataTransferObjects.RetrievalDtos;
using NumarisConnectt.Application.Services.Interfaces;


namespace DeployService
{
    public class NumarisWorker : BackgroundService
    {
        private readonly ILogger<NumarisWorker> _logger;
        private readonly DeployAccess _deployAccess;
        private readonly IServiceProvider _serviceProvider;


        public NumarisWorker(IServiceProvider serviceProvider, ILogger<NumarisWorker> logger)
        {
            _deployAccess = new DeployAccess("172.24.70.8");
            _serviceProvider = serviceProvider;
            _logger = logger;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Checking for new requests at: {time}", DateTime.Now);
                var newRequestDtos = await GetRequestsFromApiAsync();
                foreach (var request in newRequestDtos)
                {
                    var installNumarisTask = await InstallNumaris(request);
                    if (installNumarisTask.IsCompleted)
                    {
                        request.Status = "Completed";
                        await UpdateInstallationStatus(request);

                        ActionResult<UserDto> user;
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                            var userController = new UserController(userService);
                            user = await userController.GetUserById(request.UserId);
                            Mailer.SendEmail(user.Value.Email);
                        }

                    }

                }

            }
        }

        /// <summary>
        /// Read NEW requests from API
        /// </summary>
        /// <returns></returns>
        private async Task<IEnumerable<RequestDto>> GetRequestsFromApiAsync()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var requestService = scope.ServiceProvider.GetRequiredService<IRequestService>();
                    var requestController = new RequestController(requestService);
                    var result = await requestController.GetAllRequests();
                    if (result.Result is OkObjectResult okObjectResult)
                    {
                        var requests = okObjectResult.Value as IEnumerable<RequestDto>;
                        if (requests != null)
                        {
                            var newRequests = requests.Where(r => r.Status == "New").ToList();
                            return newRequests ?? new List<RequestDto>();
                        }
                    }

                    _logger.LogError("Failed to retrieve requests from the repository.");
                    return new List<RequestDto>();
                }
            }
            catch (Exception e)
            {
                throw e;
            }



        }

        /// <summary>
        /// Update Installation Status
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task UpdateInstallationStatus(RequestDto request)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var requestService = scope.ServiceProvider.GetRequiredService<IRequestService>();
                var requestController = new RequestController(requestService);
                var updateRequest = await requestController.UpdateRequest(request);
                if (updateRequest is OkObjectResult okObjectResult)
                {
                    var updatedRequestDto = okObjectResult.Value as RequestDto;
                    _logger.LogInformation("Request status updated successfully: {0}", updatedRequestDto?.Id);
                }
                else
                {
                    _logger.LogError("Failed to update request status.");
                }
            }
        }

        /// <summary>
        /// Install Numaris
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<Task> InstallNumaris(RequestDto request)
        {
            ActionResult<BaseLineDto> baseline;
            ActionResult<HostDto> host;
            ActionResult<ScannerDto> scanner;
            using (var scope = _serviceProvider.CreateScope())
            {
                var baseLineService = scope.ServiceProvider.GetRequiredService<IBaseLineService>();
                var baselineController = new BaselineControllers(baseLineService);
                baseline = await baselineController.GetBaselineById(request.BaselineId);
                if (baseline.Result is OkObjectResult okResult)
                {
                    baseline = (BaseLineDto)okResult.Value!;
                }

                var hostService = scope.ServiceProvider.GetRequiredService<IHostService>();
                var hostController = new HostController(hostService);
                host = await hostController.GetHostById(request.HostId);
                if (host.Result is OkObjectResult okHostResult)
                {
                    host = (HostDto)okHostResult.Value!;
                }

                var scannerService = scope.ServiceProvider.GetRequiredService<IScannerService>();
                var scannerController = new ScannerController(scannerService);
                scanner = await scannerController.GetScannerById(request.ScannerId);
                if (scanner.Result is OkObjectResult okScannerResult)
                {
                    scanner = (ScannerDto)okScannerResult.Value!;
                }
            };
            if (host.Value != null && baseline.Value != null && scanner.Value != null)
                _deployAccess.CreateUnattendedDeployment(host.Value, baseline.Value, scanner.Value,
                    DeployAccess.DeploymentPackageType.MRAWP, true,
                    null, null, DeployAccess.SystemVariant.ExFactory);
            return Task.CompletedTask;
        }
    }
}

