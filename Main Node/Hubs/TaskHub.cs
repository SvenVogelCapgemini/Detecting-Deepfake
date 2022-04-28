using Main_Node.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace SignalRChat.Hubs;

public class TaskHub : Hub
{
    
    private static int _userCount;
    public int UserCount => _userCount;

    private async Task SendTask(string id, string url)
    {
        await Clients.All.SendAsync("Task", id, url);
    }

    public async Task SendStatus(string id, string status)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TaskContext>();
        optionsBuilder.UseSqlite("Data Source=TaskDB.db;");
        var db = new TaskContext(optionsBuilder.Options);
        Debug.WriteLine(status);
        using (db)
        {
            var task = db.Task.Where(d => d.Id == int.Parse(id)).First();
            task.Status = status;
            db.SaveChanges();
        }
    }

    public async Task SendResult(string id, string result)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TaskContext>();
        optionsBuilder.UseSqlite("Data Source=TaskDB.db;");
        var db = new TaskContext(optionsBuilder.Options);
        Debug.WriteLine(result);
        using (db)
        {
            var task = db.Task.Where(d => d.Id == int.Parse(id)).First();
            task.Result = result;
            db.SaveChanges();
        }
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