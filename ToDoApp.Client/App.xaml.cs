using System.Windows;
using ToDoApp.Client.Services;

namespace ToDoApp.Client;

public partial class App : Application
{
    internal const string DatabaseName = "database.db";

    protected override void OnStartup(StartupEventArgs e)
    {
        DbManager.GetInstance(DatabaseName).Database.EnsureCreated();
        base.OnStartup(e);
    }
}