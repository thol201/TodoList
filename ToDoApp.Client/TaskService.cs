using ToDoApp.Client.Repositories;
using ToDoApp.Data.Models;

namespace ToDoApp.Client;

public interface ITaskService
{
    public Task<ToDoTask> Create(ToDoTask newTask);
    public Task<bool> Delete(ToDoTask taskToRemove);
    public Task<ToDoTask> Modify(ToDoTask taskToModify);
    public Task<List<ToDoTask>> GetAll();
}

public class TaskService(IToDoTaskRepo repo) : ITaskService
{
    public async Task<ToDoTask> Create(ToDoTask newTask)
    {
        newTask.CreatedAt ??= DateTime.UtcNow;
        var created = await repo.CreateAndSaveChanges(newTask);
        return created;
    }

    public async Task<bool> Delete(ToDoTask taskToRemove)
    {
        var deleted = await repo.DeleteAndSaveChanges(taskToRemove.Id);
        return deleted;
    }

    public async Task<ToDoTask> Modify(ToDoTask taskToModify)
    {
        return await repo.UpdateAndSaveChanges(taskToModify);
    }

    public async Task<List<ToDoTask>> GetAll()
    {
        return await repo.GetAll();
    }
}