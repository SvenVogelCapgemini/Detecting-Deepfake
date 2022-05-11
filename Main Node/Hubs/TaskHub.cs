using Main_Node.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Main_Node.Workers;

namespace SignalRChat.Hubs;

public class TaskHub : Hub
{
    private WorkerController _workerController = WorkerController.Instance();
    
    public async Task ReceiveStatus(string id, string status)
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

    //recieves the status of the 
    public async Task ReceiveResult(string id, string result)
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
        Debug.WriteLine(Context.ConnectionId);
        Worker worker = new Worker(Context.ConnectionId);
        Clients.Clients(worker.Id).SendAsync("ReceiveUserCount", _workerController.WorkerCount);
        _workerController.AddWorker(worker);
        Clients.All.SendAsync("ReceiveUserCount", _workerController.WorkerCount);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var worker = _workerController.GetWorker(Context.ConnectionId);
            _workerController.RemoveWorker(worker);
            Clients.All.SendAsync("ReceiveUserCount", _workerController.WorkerCount);
        }
        catch (Exception)
        {
            Debug.Fail("No id found");
        }
        return base.OnDisconnectedAsync(exception);
    }
}