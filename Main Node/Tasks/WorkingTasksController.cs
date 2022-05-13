using System.Diagnostics;
using Main_Node.Data;
using Main_Node.Workers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRChat.Hubs;

namespace Main_Node.Tasks
{
    public class WorkingTasksController
    {
        public List<Models.Task> Tasks;
        private static object locker = new object();
        static WorkingTasksController instance;
        public int TaskCount => Tasks.Count;

        protected WorkingTasksController()
        {
            Tasks = new List<Models.Task>();
            var optionsBuilder = new DbContextOptionsBuilder<TaskContext>();
            optionsBuilder.UseSqlite("Data Source=TaskDB.db;");
            var db = new TaskContext(optionsBuilder.Options);
            using (db)
            {
                var dbtasks = db.Task.Where(d => d.Status != "Done" || d.Status != "Failed").ToList();
                foreach (var task in dbtasks)
                {
                    task.Status = "Failed";
                    db.SaveChanges();
                }

            }  
        }

        public static WorkingTasksController Instance()
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new WorkingTasksController();
                    }
                }
            }
            return instance;
        }

        public void AddTask(IHubContext<TaskHub> hubContext, Models.Task task)
        {
            Tasks.Add(task);
            task.Worker = WorkerController.Instance().SendTaskToRandomWorker(hubContext, task).Result;
        }             


        public void TaskDone(int id, string result)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TaskContext>();
            optionsBuilder.UseSqlite("Data Source=TaskDB.db;");
            var db = new TaskContext(optionsBuilder.Options);
            Debug.WriteLine(result);
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
}
