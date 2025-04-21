using NumarisConnectt.Application.DataTransferObjects.RetrievalDtos;
using NumarisConnectt.Application.Services.Interfaces;


namespace DeployService.Services
{
    public class RequestStatusUpdater
    {
        private readonly IRequestService _requestService;
        private readonly ILogger<RequestStatusUpdater> _logger;

        public RequestStatusUpdater(IRequestService requestService, ILogger<RequestStatusUpdater> logger)
        {
            _requestService = requestService;
            _logger = logger;
        }

        public async Task UpdateRequestStatusAsync(RequestDto request, string status)
        {
            request.Status = status;
            var updateResult = await _requestService.UpdateRequestAsync(request);

            if (updateResult != null)
            {
                _logger.LogInformation("Request status updated successfully: {0}", request.Id);
            }
            else
            {
                _logger.LogError("Failed to update request status.");
            }
        }
    }

}
