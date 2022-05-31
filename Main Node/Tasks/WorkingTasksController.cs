using System.Diagnostics;
using Main_Node.Data;
using Main_Node.Models;
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
        if (task is MultipleTasks multipleTasks)
        {
            foreach (var task1 in multipleTasks.Tasks)
            {
                var subtask = (SubTask)task1;
                Tasks.Add(subtask);
                subtask.Worker = WorkerController.Instance().SendTaskToRandomWorker(hubContext, subtask).Result;
            }
        }
        else if (task is SingleTask singleTask)
        {
            singleTask.Worker = WorkerController.Instance().SendTaskToRandomWorker(hubContext, singleTask).Result;
        }

    }

    // used to complete a MultipleTasks
    public void MultipleTaskDone(MultipleTasks task)
    {
        var resultsstrings = task.Tasks.Where(t => t.Result != null).Select(t => t.Result).ToList();
        var results = new List<float>();
        foreach (var result in resultsstrings)
        {
            results.Add(float.Parse(result));
        }
        task.Result = results.Average().ToString();
        
        var optionsBuilder = new DbContextOptionsBuilder<TaskContext>();
        optionsBuilder.UseSqlite("Data Source=TaskDB.db;");
        var db = new TaskContext(optionsBuilder.Options);
        using (db)
        {
            var taskdb = db.Task.Where(d => d.Id == task.Id).First();
            taskdb.Result = task.Result;
            db.SaveChanges();
        }
        if (task.Tasks.All(t => t.Result != null))
        {
            optionsBuilder = new DbContextOptionsBuilder<TaskContext>();
            optionsBuilder.UseSqlite("Data Source=TaskDB.db;");
            db = new TaskContext(optionsBuilder.Options);
            using (db)
            {
                var taskdb = db.Task.Where(d => d.Id == task.Id).First();
                taskdb.Status = "Done";
                db.SaveChanges();
            }
        }
    }

    // used to complete a SingleTask and SubTask
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
            task.Result = result;
            if (task is SubTask)
            {
                var subtask = (SubTask)task;
                subtask.Worker.TaskDone();
                MultipleTaskDone((MultipleTasks) subtask.ParentTask);
            }
            else if (task is SingleTask)
            {
                var singletask = (SingleTask)task;
                singletask.Worker.TaskDone();
            }
    }
}