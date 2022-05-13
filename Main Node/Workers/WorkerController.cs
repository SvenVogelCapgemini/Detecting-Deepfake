using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using SignalRChat.Hubs;

namespace Main_Node.Workers
{
    public class WorkerController
    {
        private Random random = new Random();
        public List<Worker> workers;
        private static object locker = new object();
        static WorkerController instance;
        public int WorkerCount => workers.Count;
        public int AvailableWorkers => workers.Count(i => i.State == State.Idle);

        protected WorkerController()
        {
            workers = new List<Worker>();
        }

        public static WorkerController Instance()
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new WorkerController();
                    }
                }
            }
            return instance;
        }



        public async Task<Worker?> SendTaskToRandomWorker(IHubContext<TaskHub> hub, Models.Task task)
        {
            while (AvailableWorkers < 1)
            {
                await Task.Delay(1000);
            }
            var r = random.Next(AvailableWorkers);
            Debug.WriteLine(r);
            var worker = await workers.Where(i => i.State == State.Idle).ToArray()[r].SendTask(hub, task);
            return worker;
        }

        public void TaskDone(Models.Task task)
        {
            task.Worker.TaskDone();
        }

        public void AddWorker(Worker worker)
        {
            workers.Add(worker);
        }

        public Worker GetWorker(string id)
        {
            foreach (var worker in workers)
            {
                if (worker.Id == id)
                    return worker;
            }

            throw new ArgumentException("Given Id not found", id);
        }

        public void RemoveWorker(Worker worker)
        {
            workers.Remove(worker);
        }
    }
}
