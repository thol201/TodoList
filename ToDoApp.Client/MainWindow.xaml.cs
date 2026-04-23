using System.Windows;
using ToDoApp.Client.Repositories;
using ToDoApp.Client.Services;
using ToDoApp.Client.Validation;
using ToDoApp.Client.ViewModel;

namespace ToDoApp.Client;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new ToDoViewModel(new TaskService(new ToDoTaskRepo(DbManager.GetInstance(App.DatabaseName))), new TaskValidator());
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        ((ToDoViewModel)DataContext).OnLoadedAsync().Wait();
    }
}