using Main_Node.Workers;

namespace Main_Node.Models;

public class SubTask : Task
{
    public Task ParentTask;
    public Worker? Worker;
}