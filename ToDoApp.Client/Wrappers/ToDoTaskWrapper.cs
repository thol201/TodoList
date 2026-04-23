using ToDoApp.Data.Models;
using ToDoApp.Wpf;
using TaskStatus = ToDoApp.Data.Models.TaskStatus;

namespace ToDoApp.Client.Wrappers;

public class ToDoTaskWrapper : BindableObject
{
    private ToDoTask OriginalObject { get; }

    public ToDoTaskWrapper()
    {
        OriginalObject = new ToDoTask();
    }

    public ToDoTaskWrapper(ToDoTask task)
    {
        ArgumentNullException.ThrowIfNull(task);
        OriginalObject = task;
    }

    public int Id
    {
        get => OriginalObject.Id;
        set
        {
            OriginalObject.Id = value;
            OnPropertyChanged();
        }
    }

    public DateTime? CreatedAt
    {
        get => OriginalObject.CreatedAt;
        set
        {
            OriginalObject.CreatedAt = value;
            OnPropertyChanged();
        }
    }

    public DateTime? DueDate
    {
        get => OriginalObject.DueDate;
        set
        {
            OriginalObject.DueDate = value;
            OnPropertyChanged();
        }
    }

    public string? Title
    {
        get => OriginalObject.Title;
        set
        {
            OriginalObject.Title = value;
            OnPropertyChanged();
        }
    }

    public string? Description
    {
        get => OriginalObject.Description;
        set
        {
            OriginalObject.Description = value;
            OnPropertyChanged();
        }
    }

    public string? Author
    {
        get => OriginalObject.Author;
        set
        {
            OriginalObject.Author = value;
            OnPropertyChanged();
        }
    }

    public TaskStatus Status
    {
        get => OriginalObject.Status;
        set
        {
            OriginalObject.Status = value;
            OnPropertyChanged();
        }
    }

    public ToDoTask GetModel()
    {
        return OriginalObject;
    }
}