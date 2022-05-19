using System.Diagnostics;
using Main_Node.Data;
using Main_Node.Workers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRChat.Hubs;
using Task = Main_Node.Models.Task;

namespace Main_Node.Tasks;

public class WorkingTasksController
{
    private static readonly object locker = new();
    private static WorkingTasksController instance;
    public List<Task> Tasks;

    protected WorkingTasksController()
    {
        Tasks = new List<Task>();
        var optionsBuilder = new DbContextOptionsBuilder<TaskContext>();
        optionsBuilder.UseSqlite("Data Source=TaskDB.db;");
        var db = new TaskContext(optionsBuilder.Options);
        using (db)
        {
            var dbtasks = db.Task.Where(d => d.Status != "Done" && d.Status != "Failed").ToList();
            foreach (var task in dbtasks)
            {
                task.Status = "Failed";
                db.SaveChanges();
            }
        }
    }

    public int TaskCount => Tasks.Count;

    public static WorkingTasksController Instance()
    {
        if (instance == null)
            lock (locker)
            {
                if (instance == null) instance = new WorkingTasksController();
            }

        return instance;
    }

    public void AddTask(IHubContext<TaskHub> hubContext, Task task)
    {
        Tasks.Add(task);
        task.Worker = WorkerController.Instance().SendTaskToRandomWorker(hubContext, task).Result;
    }


    public void TaskDone(int id, string result)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TaskContext>();
        optionsBuilder.UseSqlite("Data Source=TaskDB.db;");
        var db = new TaskContext(optionsBuilder.Options);
        using (db)
        {
            var taskdb = db.Task.Where(d => d.Id == id).First();
            taskdb.Result = result;
            db.SaveChanges();
        }

        var task = Tasks.Where(t => t.Id == id).First();
        task.Worker.TaskDone();
    }
}