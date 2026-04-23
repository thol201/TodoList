using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ToDoApp.Client.Validation;
using ToDoApp.Client.Wrappers;
using ToDoApp.Wpf;
using TaskState = ToDoApp.Data.Models.TaskStatus;

namespace ToDoApp.Client.ViewModel;

public class ToDoViewModel(ITaskService taskService, ITaskValidator taskValidator) : BindableObject
{
    private ObservableCollection<ToDoTaskWrapper> _tasks = [];
    private ICollectionView? _filteredTasks;
    private DateTime _selectedDate = DateTime.Now;
    private ToDoTaskWrapper _newTask = CreateEmptyTask();

    public StatusBarViewModel StatusBar { get; } = new();

    #region commands

    public ICommand PreviousDay => new RelayCommand(() =>
    {
        SelectedDate = SelectedDate.AddDays(-1);
    });

    public ICommand NextDay => new RelayCommand(() =>
    {
        SelectedDate = SelectedDate.AddDays(1);
    });

    public ICommand AddTask => new AsyncCommand<object?>(async _ =>
    {
        var validationResult = taskValidator.Validate(NewTask);
        if (!validationResult.IsValid)
        {
            await StatusBar.ShowTemporaryErrorAsync(validationResult.GetFirstError().ErrorMessage);
            return;
        }

        try
        {
            var created = await taskService.Create(NewTask.GetModel());
            Tasks.Add(new ToDoTaskWrapper(created));
            NewTask = CreateEmptyTask();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await StatusBar.ShowTemporaryErrorAsync("Failed to add task");
        }
    });

    public ICommand RemoveTask => new AsyncCommand<ToDoTaskWrapper>(async task =>
    {
        if (task is null)
            return;

        try
        {
            var deleted = await taskService.Delete(task.GetModel());
            if (!deleted)
            {
                await StatusBar.ShowTemporaryErrorAsync("Task could not be removed.");
                return;
            }

            Tasks.Remove(task);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await StatusBar.ShowTemporaryErrorAsync("Failed to remove task");
        }
    });

    public ICommand ModifyTask => new AsyncCommand<ToDoTaskWrapper>(async task =>
    {
        if (task is null)
            return;

        try
        {
            await taskService.Modify(task.GetModel());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await StatusBar.ShowTemporaryErrorAsync("Failed to update task");
        }
    });

    #endregion

    public DateTime SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (SetField(ref _selectedDate, value))
            {
                RefreshFilteredTasks();
                UpdateReminderStatusMessage();
            }
        }
    }
    public ToDoTaskWrapper NewTask
    {
        get => _newTask;
        set => SetField(ref _newTask, value);
    }

    private ObservableCollection<ToDoTaskWrapper> Tasks
    {
        get => _tasks;
        set
        {
            if (SetField(ref _tasks, value))
            {
                InitializeFilteredTasksView();
                RefreshFilteredTasks();
                UpdateReminderStatusMessage();
            }
        }
    }

    public ICollectionView? FilteredTasks
    {
        get => _filteredTasks;
        private set => SetField(ref _filteredTasks, value);
    }

    public IReadOnlyList<TaskState> TaskStatuses { get; } = Enum.GetValues<TaskState>();

    public async Task OnLoadedAsync()
    {
        InitializeFilteredTasksView();
        var tasks = await taskService.GetAll();
        Tasks = new ObservableCollection<ToDoTaskWrapper>(tasks.Select(x => new ToDoTaskWrapper(x)));
    }

    private void RefreshFilteredTasks()
    {
        FilteredTasks?.Refresh();
    }

    private void InitializeFilteredTasksView()
    {
        var view = CollectionViewSource.GetDefaultView(Tasks);
        view.Filter = item => item is ToDoTaskWrapper task && task.DueDate?.Date == SelectedDate.Date;
        FilteredTasks = view;
    }

    private static ToDoTaskWrapper CreateEmptyTask()
    {
        var now = DateTime.Now;

        return new ToDoTaskWrapper
        {
            CreatedAt = now,
            DueDate = now.Date,
            Status = TaskState.Pending
        };
    }

    private void UpdateReminderStatusMessage()
    {
        var tomorrow = DateTime.Today.AddDays(1);
        var notCompletedForTomorrow = Tasks.Count(task =>
            task.DueDate?.Date == tomorrow && task.Status != TaskState.Completed);

        StatusBar.SetReminderMessage(notCompletedForTomorrow > 0
            ? $"{notCompletedForTomorrow} {(notCompletedForTomorrow == 1 ? "task" : "tasks")} due tomorrow"
            : string.Empty);
    }

}