#nullable disable
using Main_Node.Models;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Main_Node.Data;

public class TaskContext : DbContext
{
    public TaskContext(DbContextOptions<TaskContext> options)
        : base(options)
    {
    }

    public DbSet<Models.Task> Task { get; set; }
    public DbSet<Models.SubTask> SubTask { get; set; }
    public DbSet<Models.MultipleTasks> MultipleTask { get; set; }
    public DbSet<Models.SingleTask> SingleTask { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=TaskDB.db;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }
}