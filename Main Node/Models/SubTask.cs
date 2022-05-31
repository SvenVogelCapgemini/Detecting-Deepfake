using Main_Node.Workers;

namespace Main_Node.Models
{
    public class SubTask : Task
    {
        public Worker? Worker;
        public Task ParentTask;
    }
}
