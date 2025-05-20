using NumarisConnectt.Application.DataTransferObjects.RetrievalDtos;
using NumarisConnectt.Application.Services.Interfaces;


namespace DeployService.Utilities
{
    public class RequestProcessor
    {

        private readonly IUserService _userService;
        private readonly IScannerService _scannerService;
        private readonly IHostService _hostService;
        private readonly IBaseLineService _baselineService;
        private readonly IRequestService _requestService;

        public RequestProcessor(
            IUserService userService,
            IScannerService scannerService,
            IHostService hostService,
            IBaseLineService baselineService,
            IRequestService requestService)
        {
            _userService = userService;
            _scannerService = scannerService;
            _hostService = hostService;
            _baselineService = baselineService;
            _requestService = requestService;
        }

        public async Task<IEnumerable<RequestDto>> GetNewRequestsAsync()
        {
            var result = await _requestService.GetAllRequestsAsync();
            return result.IsSuccess
                ? result.Value.Where(r => r.Status == Status.New).ToList()
                : new List<RequestDto>();
        }

        public async Task<(UserDto? User, HostDto? Host, ScannerDto? Scanner, BaseLineDto? Baseline)> RetrieveRequestDataAsync(RequestDto request)
        {
            var user = (await _userService.GetUserByIdAsync(request.UserId)).Value;
            var scanner = (await _scannerService.GetScannerByIdAsync(request.ScannerId)).Value;
            var host = (await _hostService.GetHostByIdAsync(request.HostId)).Value;
            var baseline = (await _baselineService.GetBaseLineByIdAsync(request.BaselineId)).Value;

            return (user, host, scanner, baseline);
        }
    }
}
