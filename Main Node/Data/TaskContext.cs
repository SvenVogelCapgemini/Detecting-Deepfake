#nullable disable
using Microsoft.EntityFrameworkCore;
using Task = Main_Node.Models.Task;

namespace Main_Node.Data;

public class TaskContext : DbContext
{
    public TaskContext(DbContextOptions<TaskContext> options)
        : base(options)
    {
    }

    public DbSet<Task> Task { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=TaskDB.db;");
    }
}