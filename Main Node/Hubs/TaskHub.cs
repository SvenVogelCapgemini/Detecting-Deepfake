using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs;

public class TaskHub : Hub
{
    private static int _userCount;
    public int UserCount => _userCount;

    private async Task SendTask(string id, string url)
    {
        await Clients.All.SendAsync("Task", id, url);
    }

    public async Task Result()
    {
        await Clients.Caller.SendAsync("ReceiveUserCount", _userCount);
    }

    public override Task OnConnectedAsync()
    {
        _userCount++;
        Clients.All.SendAsync("ReceiveUserCount", _userCount);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _userCount--;
        Clients.All.SendAsync("ReceiveUserCount", _userCount);
        return base.OnDisconnectedAsync(exception);
    }
}