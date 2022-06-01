using System.Diagnostics;
using Main_Node.Data;
using Main_Node.Tasks;
using Main_Node.Workers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace SignalRChat.Hubs;

public class TaskHub : Hub
{
    private readonly WorkerController _workerController = WorkerController.Instance();

    /// <summary>
    ///     Receive the status of the task from the worker.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="status"></param>
    public void ReceiveStatus(int id, string status)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TaskContext>();
        optionsBuilder.UseSqlite("Data Source=TaskDB.db;");
        var db = new TaskContext(optionsBuilder.Options);
        using (db)
        {
            var task = db.Task.Where(d => d.Id == id).First();
            task.Status = status;
            db.SaveChanges();
        }
    }

    /// <summary>
    ///     Receive the result of the task from the worker.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="result"></param>
    public void ReceiveResult(int id, string result)
    {
        WorkingTasksController.Instance().TaskDone(id, result);
    }

    /// <summary>
    ///     Gets called when a worker connects to the Hub.
    /// </summary>
    /// <returns></returns>
    public override Task OnConnectedAsync()
    {
        var worker = new Worker(Context.ConnectionId);
        _workerController.AddWorker(worker);
        Clients.All.SendAsync("ReceiveUserCount", _workerController.WorkerCount);
        return base.OnConnectedAsync();
    }

    /// <summary>
    ///     Gets called when a worker disconnects.
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
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