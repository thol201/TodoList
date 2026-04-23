using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Data.Models;

public enum TaskStatus
{
    InProgress,
    Completed,
    Pending
}

public class ToDoTask
{
    [Key]
    public int Id { get; set; } 
    public DateTime? CreatedAt { get; set; } 
    public DateTime? DueDate { get; set; } 
    [MaxLength(100)] public string? Title { get; set; }
    [MaxLength(100)] public string? Description { get; set; }
    [MaxLength(100)] public string? Author { get; set; }
    public TaskStatus Status { get; set; }
}