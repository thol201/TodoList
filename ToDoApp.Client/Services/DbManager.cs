using System.IO;
using ToDoApp.Data;

namespace ToDoApp.Client.Services;

public static class DbManager
{
    private static readonly Dictionary<string, Lazy<AppDbContext>> Instances = new(StringComparer.OrdinalIgnoreCase);

    public static AppDbContext GetInstance(string dbName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dbName);

        lock (Instances)
        {
            if (Instances.TryGetValue(dbName, out var instance)) return instance.Value;
            instance = new Lazy<AppDbContext>(() =>
            {
                var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TodoList");
                Directory.CreateDirectory(appDataPath);
                var fullDbPath = Path.Combine(appDataPath, dbName);

                return new AppDbContext(fullDbPath);
            });
            Instances[dbName] = instance;

            return instance.Value;
        }
    }
}
