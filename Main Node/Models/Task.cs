using System.ComponentModel.DataAnnotations;

namespace Main_Node.Models;

public abstract class Task
{
    [Key] public int Id { get; set; }

    [Required(ErrorMessage = "URL Required")]
    [Url]
    public string URL { get; set; }

    [Required(ErrorMessage = "Methode Required")]
    public Method Method { get; set; }

    [Required(ErrorMessage = "Status Required")]
    public string Status { get; set; }

    public string? Result { get; set; }
}

public enum Method
{
    AllwaysTrue = 1,
    AllwaysFalse = 2,
    XceptionNet = 3,
    RunAll = 4
}