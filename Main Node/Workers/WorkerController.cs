using System.Diagnostics;
using Main_Node.Models;
using Microsoft.AspNetCore.SignalR;
using SignalRChat.Hubs;
using Task = Main_Node.Models.Task;

namespace Main_Node.Workers;

public class WorkerController
{
    private static readonly object locker = new();
    private static WorkerController instance;
    private readonly Random random = new();
    public List<Worker> workers;

    protected WorkerController()
    {
        workers = new List<Worker>();
    }

    public int WorkerCount => workers.Count;
    public int AvailableWorkers => workers.Count(i => i.State == State.Idle);

    public static WorkerController Instance()
    {
        if (instance == null)
            lock (locker)
            {
                if (instance == null) instance = new WorkerController();
            }

        return instance;
    }


    public async Task<Worker?> SendTaskToRandomWorker(IHubContext<TaskHub> hub, Task task)
    {
        while (AvailableWorkers < 1) await System.Threading.Tasks.Task.Delay(1000);
        var r = random.Next(AvailableWorkers);
        Debug.WriteLine(r);
        var worker = await workers.Where(i => i.State == State.Idle).ToArray()[r].SendTask(hub, task);
        return worker;
    }

    public void TaskDone(Task task)
    {
        if (task is SubTask)
        {
            var subTask = (SubTask)task;
            subTask.Worker.TaskDone();
        }
        else if (task is SingleTask)
        {
            var singleTask = (SingleTask)task;
            singleTask.Worker.TaskDone();
        }
    }

    public void AddWorker(Worker worker)
    {
        workers.Add(worker);
    }

    public Worker GetWorker(string id)
    {
        foreach (var worker in workers)
            if (worker.Id == id)
                return worker;

        throw new ArgumentException("Given Id not found", id);
    }

    public void RemoveWorker(Worker worker)
    {
        workers.Remove(worker);
    }
}