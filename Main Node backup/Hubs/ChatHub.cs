using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs;

public class ChatHub : Hub
{
    private static int _userCount;
    public int UserCount => _userCount;

    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task GetUserCount()
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