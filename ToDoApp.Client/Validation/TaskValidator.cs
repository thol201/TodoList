using ToDoApp.Client.Wrappers;

namespace ToDoApp.Client.Validation;

public interface ITaskValidator
{
    ValidationResult Validate(ToDoTaskWrapper task);
}

public class TaskValidator : ITaskValidator
{
    private static readonly ValidationRule[] Rules =
    [
        new(nameof(ToDoTaskWrapper.Title), task => !string.IsNullOrWhiteSpace(task.Title), "Title cannot be empty."),
        new(nameof(ToDoTaskWrapper.Description), task => !string.IsNullOrWhiteSpace(task.Description), "Description cannot be empty."),
        new(nameof(ToDoTaskWrapper.Author), task => !string.IsNullOrWhiteSpace(task.Author), "Author cannot be empty."),
        new(nameof(ToDoTaskWrapper.DueDate), task => task.DueDate is not null, "Due date cannot be empty.")
    ];

    public ValidationResult Validate(ToDoTaskWrapper task)
    {
        ArgumentNullException.ThrowIfNull(task);

        var errors = Rules
            .Where(rule => !rule.IsValid(task))
            .Select(rule => new ValidationFailure(rule.PropertyName, rule.ErrorMessage))
            .ToList();

        return errors.Count == 0
            ? ValidationResult.FromSuccess()
            : ValidationResult.FromError(errors);
    }

    private record ValidationRule(string PropertyName, Func<ToDoTaskWrapper, bool> IsValid, string ErrorMessage);
}

public record ValidationFailure(string PropertyName, string ErrorMessage);

public class ValidationResult
{
    private ValidationResult(IReadOnlyList<ValidationFailure> errors)
    {
        Errors = errors;
    }

    private IReadOnlyList<ValidationFailure> Errors { get; }

    public bool IsValid => Errors.Count == 0;

    public ValidationFailure GetFirstError() => Errors[0];

    public static ValidationResult FromSuccess() => new([]);

    public static ValidationResult FromError(IReadOnlyList<ValidationFailure> errors) => new(errors);
}
