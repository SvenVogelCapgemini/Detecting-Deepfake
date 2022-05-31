#nullable disable
using System.Diagnostics;
using Main_Node.Data;
using Main_Node.Models;
using Main_Node.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRChat.Hubs;
using Task = Main_Node.Models.Task;

namespace Main_Node.Controllers;

public class TaskController : Controller
{
    private readonly TaskContext _context;
    private readonly IHubContext<TaskHub> _hubContext;

    public TaskController(TaskContext context, IHubContext<TaskHub> hubContext)
    {
        _hubContext = hubContext;
        _context = context;
        WorkingTasksController.Instance();
    }

    // GET: Task
    public async Task<IActionResult> Index()
    {
        var list = await _context.Task.ToListAsync();
        var subtask = list.Where(t => t is not SubTask);
        return View(subtask);
    }

    // GET: Task/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var task = await _context.Task
            .FirstOrDefaultAsync(m => m.Id == id);
        if (task == null) return NotFound();
        if (task is MultipleTasks)
        {
            var multipleTasks = await _context.MultipleTask.Include(mt => mt.Tasks).FirstOrDefaultAsync(m => m.Id == id);

            return View(multipleTasks);
        }

        return View(task);
    }

    // GET: Task/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Task/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string URL, Method method)
    {
        Task task = null;
        if (method == Method.RunAll)
        {
            var status = "Waiting for worker";
            MultipleTasks parent = new MultipleTasks()
            {
                Method = method,
                URL = URL,
                Status = status,
                Tasks = new List<Task>()
            };
            Task allwaysTrue = new SubTask()
            {
                URL = URL,
                Method = Method.AllwaysTrue,
                Status = status,
                ParentTask = parent
            };
            Task allwaysFalse = new SubTask()
            {
                URL = URL,
                Method = Method.AllwaysFalse,
                Status = status,
                ParentTask = parent
            }; 
            Task xceptionNet = new SubTask()
            {
                URL = URL,
                Method = Method.XceptionNet,
                Status = status,
                ParentTask = parent
            };

            parent.Tasks.Add(allwaysTrue);
            parent.Tasks.Add(xceptionNet);
            parent.Tasks.Add(allwaysFalse);
            _context.Add(allwaysTrue);
            _context.Add(allwaysFalse);
            _context.Add(xceptionNet);
            _context.Add(parent);
            task = parent;
        }
        else
        {
            var status = "Waiting for worker";
            var singleTask = new SingleTask()
            {
                Method = method,
                URL = URL,
                Status = status
            };
            _context.Add(singleTask);
            task = singleTask;
        }

        await _context.SaveChangesAsync();
        System.Threading.Tasks.Task.Run(() => WorkingTasksController.Instance().AddTask(_hubContext, task));
        return RedirectToAction("Index");
    }


    // GET: Task/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var task = await _context.Task
            .FirstOrDefaultAsync(m => m.Id == id);
        if (task == null) return NotFound();

        return View(task);
    }

    // POST: Task/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var task = await _context.Task.FindAsync(id);
        _context.Task.Remove(task);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}