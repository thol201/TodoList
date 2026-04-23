using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ToDoApp.Wpf;

public sealed class AsyncCommand<T>(Func<T?, CancellationToken, Task> execute) : IAsyncRelayCommand<T>
{
    private readonly Func<T?, CancellationToken, Task> _execute = execute ?? throw new ArgumentNullException(nameof(execute));
    private readonly CancellationTokenSource _cts = new();
    private Task? _executionTask;
    private bool _isRunning;

    public AsyncCommand(Func<T?, Task> execute)
        : this((param, token) => execute(param))
    {
    }

    public Task? ExecutionTask
    {
        get => _executionTask;
        private set
        {
            _executionTask = value;
            OnPropertyChanged(nameof(ExecutionTask));
        }
    }

    public bool IsRunning
    {
        get => _isRunning;
        private set
        {
            _isRunning = value;
            OnPropertyChanged(nameof(IsRunning));
        }
    }

    public bool CanBeCanceled => IsRunning;
    public bool IsCancellationRequested => _cts.IsCancellationRequested;

    public async Task ExecuteAsync(T? parameter)
    {
        IsRunning = true;
        ExecutionTask = _execute(parameter, _cts.Token);

        try
        {
            await ExecutionTask;
        }
        finally
        {
            IsRunning = false;
        }
    }

    public bool CanExecute(T? parameter) => true;
    public bool CanExecute(object? parameter) => true;

    public async void Execute(object? parameter)
    {
        try
        {
            await ExecuteAsync((T?)parameter);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void Execute(T? parameter) => Execute((object?)parameter);
    public Task ExecuteAsync(object? parameter) => ExecuteAsync((T?)parameter);

    public void Cancel() => _cts.Cancel();

    public event EventHandler? CanExecuteChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    private void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
