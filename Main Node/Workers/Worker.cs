using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using SignalRChat.Hubs;

namespace Main_Node.Workers;

public class Worker
{
    public List<Algorithm> Algorithms;
    private State _state;
    public State State => _state;
    public string Id { get; }

    public Worker(string id)
    {
        Algorithms = new List<Algorithm>();
        Id = id;
        _state = State.Idle;
    }

    public void AddAlgorithms(Algorithm algorithm)
    {
        Algorithms.Add(algorithm);
    }

    public async Task<Worker> SendTask(IHubContext<TaskHub> hub, Models.Task task)
    {
        _state = State.Busy;
        Debug.WriteLine("Send Message");
        Debug.WriteLine(Id);
        await hub.Clients.Client(Id).SendAsync("Task", task.Id.ToString(), task.URL, task.Methode);
        return this;
    }

    public void TaskDone()
    {
        _state = State.Idle;
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