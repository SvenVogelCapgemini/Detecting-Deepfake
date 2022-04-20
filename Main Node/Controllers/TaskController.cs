using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;
using SignalRChat.Hubs;

namespace Main_Node.Controllers
{
    public class TaskController : Controller
    {
        private readonly IHubContext<TaskHub> _hubContext;

        public TaskController(IHubContext<TaskHub> hubContext)
        {
            _hubContext = hubContext;
        }
        // GET: TaskController
        public ActionResult Index()
        {
            return View();
        }


        // GET: TaskController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TaskController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                
                collection.TryGetValue("URL", out var url);
                collection.TryGetValue("Methode", out var methode);
                Random rng = new Random();
                _hubContext.Clients.All.SendAsync("Task", GenerateString(rng, 16), url.ToString(), methode.ToString());
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public char GenerateChar(Random rng)
        {
            // 'Z' + 1 because the range is exclusive
            return (char)(rng.Next('A', 'Z' + 1));
        }

        public string GenerateString(Random rng, int length)
        {
            char[] letters = new char[length];
            for (int i = 0; i < length; i++)
            {
                letters[i] = GenerateChar(rng);
            }
            return new string(letters);
        }
    }
}
