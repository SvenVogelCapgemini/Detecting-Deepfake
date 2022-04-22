#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Main_Node.Models;

namespace Main_Node.Data
{
    public class TaskContext : DbContext
    {
        public TaskContext (DbContextOptions<TaskContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=TaskDB.db;");
        }

        public DbSet<Main_Node.Models.Task> Task { get; set; }
    }
}
