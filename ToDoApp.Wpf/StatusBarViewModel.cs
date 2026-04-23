namespace ToDoApp.Wpf;

public class StatusBarViewModel : BindableObject
{
    private string _message = string.Empty;
    private bool _isErrorStatus;
    private string _pendingMessage = string.Empty;
    private CancellationTokenSource? _errorStatusCancellation;

    public string Message
    {
        get => _message;
        private set
        {
            if (SetField(ref _message, value))
            {
                OnPropertyChanged(nameof(IsVisible));
            }
        }
    }

    public bool IsVisible => !string.IsNullOrWhiteSpace(Message);

    public bool IsErrorStatus
    {
        get => _isErrorStatus;
        private set => SetField(ref _isErrorStatus, value);
    }

    public void SetReminderMessage(string message)
    {
        _pendingMessage = message;

        if (IsErrorStatus)
        {
            return;
        }

        Message = message;
    }

    public async Task ShowTemporaryErrorAsync(string message)
    {
        if (_errorStatusCancellation is not null)
        {
            await _errorStatusCancellation.CancelAsync();
            _errorStatusCancellation.Dispose();
        }

        var cancellation = new CancellationTokenSource();
        _errorStatusCancellation = cancellation;

        IsErrorStatus = true;
        Message = message;

        try
        {
            await Task.Delay(1000, cancellation.Token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        if (!ReferenceEquals(_errorStatusCancellation, cancellation))
        {
            return;
        }

        IsErrorStatus = false;
        _errorStatusCancellation.Dispose();
        _errorStatusCancellation = null;
        Message = _pendingMessage;
    }
}
