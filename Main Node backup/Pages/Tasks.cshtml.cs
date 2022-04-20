using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRChat.Hubs;
using Task = Main_Node.Models.Task;

namespace Main_Node.Pages
{
    public class TasksModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Task Task { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Random rng = new Random();
            Task.Id = GenerateString(rng, 16);
            Task.Methode = "Test";
            return Page();
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
