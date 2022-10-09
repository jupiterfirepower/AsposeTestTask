using microservice.common;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Threading;
using System;

namespace microservice.core
{
    public class NotificationHub : Hub, INotificationHub
    {
        private readonly ILogger<NotificationHub> _logger;

        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        private string ConnectionId => Context.ConnectionId;

        public async Task PushDataAsync(Notification data, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"{nameof(PushDataAsync)} Notification - {data.ToString()}, UserId - {data.CorellationId}");
                await PushDataByUserIdAsync(data, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(PushDataAsync)} error.");
            }
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"{nameof(OnConnectedAsync)} ConnectionId - {ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"{nameof(OnDisconnectedAsync)} ConnectionId - {ConnectionId}, Exception - {exception?.Message}");
            await base.OnDisconnectedAsync(exception);
        }

        private async Task PushDataByUserIdAsync(Notification data, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(PushDataByUserIdAsync)} data.CorellationId - {data.CorellationId}");
            _logger.LogInformation($"{nameof(PushDataByUserIdAsync)} Clients - {Clients != null}");
            _logger.LogInformation($"{nameof(PushDataByUserIdAsync)} Clients.All - {Clients?.All != null}");
            //var first = Clients.Users.FirstOrDefault();
            //await Clients.Users(ConnectionId).SendAsync("nscevents", $"data:{JsonConvert.SerializeObject(data, _jsonSettings)}\n", cancellationToken);
            await Clients.All.SendAsync("nscevents", $"data:{JsonConvert.SerializeObject(data, _jsonSettings)}\n", cancellationToken);

            //await Clients.Users(data.CorellationId).SendAsync("nscevents", $"data:{JsonConvert.SerializeObject(data, _jsonSettings)}\n", cancellationToken);
        }
    }
}