using System.ComponentModel.DataAnnotations;

namespace Main_Node.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "URL Required")]
        [Url]
        public string URL { get; set; }
        [Required(ErrorMessage = "Methode Required")]
        public string Methode { get; set; }
        public string? Status { get; set; }
        public string? Result { get; set; }
    }
}
