using microservice.common;
using microservice.core;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;

namespace microservice.gateway.Services
{
    public class NotificationService: INotificationHub
    {
        private readonly ILogger<NotificationService> _logger;
        IHubContext<NotificationHub> _hubContext;

        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };

        public NotificationService(ILogger<NotificationService> logger, IHubContext<NotificationHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task PushDataAsync(Notification data, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"{nameof(PushDataAsync)} Notification CorellationId - {data.CorellationId}");
                await _hubContext.Clients.All.SendAsync("nscevents", $"{JsonConvert.SerializeObject(data, _jsonSettings)}\n", cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(PushDataAsync)} error.");
            }
        }
    }
}
