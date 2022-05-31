using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using SignalRChat.Hubs;
using Task = Main_Node.Models.Task;

namespace Main_Node.Workers;

public class Worker
{
    public List<Algorithm> Algorithms;

    public Worker(string id)
    {
        Algorithms = new List<Algorithm>();
        Id = id;
        State = State.Idle;
    }

    public State State { get; private set; }

    public string Id { get; }

    public void AddAlgorithms(Algorithm algorithm)
    {
        Algorithms.Add(algorithm);
    }

    public async Task<Worker> SendTask(IHubContext<TaskHub> hub, Task task)
    {
        State = State.Busy;
        Debug.WriteLine("Send Message");
        Debug.WriteLine(Id);
        await hub.Clients.Client(Id).SendAsync("Task", task.Id, task.URL, Convert.ToInt32(task.Method).ToString());
        return this;
    }

    public void TaskDone()
    {
        State = State.Idle;
    }
}

public struct Algorithm
{
    public int Index;
    public string Name;
}

public enum State
{
    Idle,
    Busy
}