using microservice.common;
using Microsoft.AspNetCore.SignalR;

namespace microservice.core;

public class CustomSignalRUserIdProvider : IUserIdProvider
{
    private readonly ICurrentUserService _currentUserService;

    public CustomSignalRUserIdProvider(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public string GetUserId(HubConnectionContext connection)
    {
        return connection.ConnectionId;
        //return _currentUserService.UserId;
    }
}
