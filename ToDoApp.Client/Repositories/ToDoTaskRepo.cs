using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Data.Models;

namespace ToDoApp.Client.Repositories;

public interface IToDoTaskRepo
{
    Task<List<ToDoTask>> GetAll();
    Task<ToDoTask?> GetById(int id);
    Task<ToDoTask> CreateAndSaveChanges(ToDoTask task);
    Task<ToDoTask> UpdateAndSaveChanges(ToDoTask task);
    Task<bool> DeleteAndSaveChanges(int id);
}

public class ToDoTaskRepo(AppDbContext dbContext) : IToDoTaskRepo
{
    public async Task<List<ToDoTask>> GetAll()
        => await dbContext.Tasks.ToListAsync();

    public async Task<ToDoTask?> GetById(int id)
        => await dbContext.Tasks.SingleOrDefaultAsync(x => x.Id == id);

    public async Task<ToDoTask> CreateAndSaveChanges(ToDoTask task)
    {
        var newTask = await dbContext.Tasks.AddAsync(task);
        await dbContext.SaveChangesAsync();

        return newTask.Entity;
    }

    public async Task<ToDoTask> UpdateAndSaveChanges(ToDoTask task)
    {
        var existingTask = await GetById(task.Id);

        if (existingTask is null)
        {
            throw new KeyNotFoundException($"Task with id {task.Id} was not found.");
        }

        dbContext.Entry(existingTask).CurrentValues.SetValues(task);
        await dbContext.SaveChangesAsync();

        return existingTask;
    }

    public async Task<bool> DeleteAndSaveChanges(int id)
    {
        var taskToDelete = await dbContext.Tasks.SingleOrDefaultAsync(x => x.Id == id);

        if (taskToDelete is null)
        {
            return false;
        }

        dbContext.Tasks.Remove(taskToDelete);
        await dbContext.SaveChangesAsync();

        return true;
    }
}
