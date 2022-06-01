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
            foreach (var task1 in multipleTasks.Tasks)
            {
                var subtask = (SubTask)task1;
                Tasks.Add(subtask);
                subtask.Worker = WorkerController.Instance().SendTaskToRandomWorker(hubContext, subtask).Result;
            }
        else if (task is SingleTask singleTask)
            singleTask.Worker = WorkerController.Instance().SendTaskToRandomWorker(hubContext, singleTask).Result;
    }

    /// <summary>
    ///     Updates a MultipleTask upon completion of one of the SubTasks.
    /// </summary>
    /// <param name="task"></param>
    public void MultipleTaskDone(MultipleTasks task)
    {
        // get all the results of the results that aren't null from the SubTasks
        var resultsstrings = task.Tasks.Where(t => t.Result != null).Select(t => t.Result).ToList();
        var results = new List<float>();
        // Change all the strings to floats
        foreach (var result in resultsstrings) results.Add(float.Parse(result));
        // get the average of the floats
        task.Result = results.Average().ToString();
        // Update the task in the database 
        var optionsBuilder = new DbContextOptionsBuilder<TaskContext>();
        optionsBuilder.UseSqlite("Data Source=TaskDB.db;");
        var db = new TaskContext(optionsBuilder.Options);
        using (db)
        {
            var taskdb = db.Task.Where(d => d.Id == task.Id).First();
            taskdb.Result = task.Result;
            db.SaveChanges();
        }

        // If all the SubTasks have a result mark the task as Done
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
            // Remove The Tasks from the list
            foreach (var subTask in task.Tasks) Tasks.Remove(subTask);
            Tasks.Remove(task);
        }
    }

    /// <summary>
    ///     Used to mark a SingleTask and SubTask for Completion.
    ///     Frees up the worker used for the task and if required updates the parentTask.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="result"></param>
    public void TaskDone(int id, string result)
    {
        // Update the result in the database
        var optionsBuilder = new DbContextOptionsBuilder<TaskContext>();
        optionsBuilder.UseSqlite("Data Source=TaskDB.db;");
        var db = new TaskContext(optionsBuilder.Options);
        using (db)
        {
            var taskdb = db.Task.Where(d => d.Id == id).First();
            taskdb.Result = result;
            db.SaveChanges();
        }

        // Update the task in the Tasks list 
        var task = Tasks.Where(t => t.Id == id).First();
        task.Result = result;
        if (task is SubTask subTask)
        {
            // Signal the worker that the task is done
            subTask.Worker.TaskDone();
            // Signal the parent task that the work is done
            MultipleTaskDone((MultipleTasks)subTask.ParentTask);
        }
        else if (task is SingleTask singleTask)
        {
            // Singal the worker that the task is done
            singleTask.Worker.TaskDone();
            // Remove Task from queue
            Tasks.Remove(task);
        }
    }
}