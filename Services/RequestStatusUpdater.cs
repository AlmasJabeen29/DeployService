using DeployService.Clients;
using NumarisConnectt.Application.DataTransferObjects.RetrievalDtos;


namespace DeployService.Services
{
    public class RequestStatusUpdater
    {
        private readonly ApiClient _ApiClient;
        private readonly ILogger<RequestStatusUpdater> _logger;

        public RequestStatusUpdater(ApiClient ApiClient, ILogger<RequestStatusUpdater> logger)
        {
           _ApiClient = ApiClient;
            _logger = logger;
        }

        public async Task UpdateRequestStatusAsync(RequestDto request, Status status)
        {
            request.Status = status;
            try
            {
                await _ApiClient.UpdateRequestAsync(request);
                _logger.LogInformation("Request status updated successfully: {0}", request.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update request status for request ID: {0}", request.Id);
            }

        }
    }

}
