using DeployService.Clients;
using NumarisConnectt.Application.DataTransferObjects.RetrievalDtos;


namespace DeployService.Services
{
    public class RequestStatusUpdater
    {
        private readonly RequestApiClient _requestApiClient;
        private readonly ILogger<RequestStatusUpdater> _logger;

        public RequestStatusUpdater(RequestApiClient requestApiClient, ILogger<RequestStatusUpdater> logger)
        {
           _requestApiClient = requestApiClient;
            _logger = logger;
        }

        public async Task UpdateRequestStatusAsync(RequestDto request, Status status)
        {
            request.Status = status;
            try
            {
                await _requestApiClient.UpdateRequestAsync(request);
                _logger.LogInformation($"Request status updated successfully for id {request.Id} : {request.Status.ToString()}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update request status for request ID: {0}", request.Id);
            }

        }
    }

}
