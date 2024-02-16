using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MauiMvvmDemo.ViewModels.Messages;
using MauiMvvmDemo.ViewModels.Models;
using System.Collections.ObjectModel;

namespace MauiMvvmDemo.ViewModels.Pages;

public partial class MainPageVM : ObservableObject
{
    [ObservableProperty]
    private string _username = string.Empty;

    public MainPageVM()
    {
        ResetSelectedFields(); // Initialize SelectedFields
        // Messenger can be used to communicate between pages or controls:
        // Sample for receiving message
        //WeakReferenceMessenger.Default.Register<UsernameChangedMessage>(this, (recipient, message) =>
        //{
        //    // 
        //});

        //// Sample for sending message
        //WeakReferenceMessenger.Default.Send(new UsernameChangedMessage("My new Username"));
    }

    public ICollection<string> AvailableColors { get; } = new string[]
    {
        "Red",
        "Blue",
        "Green",
        "Yellow",
        "Orange",
        "Purple"
    };

    public ObservableCollection<SelectedField> SelectedFields { get; } = new();

    public ObservableCollection<IEnumerable<SelectedField>> Moves { get; } = new();

    [RelayCommand(CanExecute = nameof(CanExecuteMakeMoveCommand))]
    private async Task MakeMoveAsync()
    {
        Moves.Add(SelectedFields.ToArray());  // ToArray to copy the values
        ResetSelectedFields();
        await Task.Delay(100); // simulates some async logic
    }

    private bool CanExecuteMakeMoveCommand() =>
        true; // e.g. validation logic here

    private void ResetSelectedFields()
    {
        SelectedFields.Clear();

        for (int i = 0; i < 4; i++)
            SelectedFields.Add(new SelectedField());
    }
}
