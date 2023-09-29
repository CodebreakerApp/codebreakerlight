using MauiMvvmDemo.ViewModels.Commands;
using MauiMvvmDemo.ViewModels.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace MauiMvvmDemo.ViewModels.Pages;

public class MainPageVM : INotifyPropertyChanging, INotifyPropertyChanged
{
    private string _username = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public event PropertyChangingEventHandler? PropertyChanging;

    public MainPageVM()
    {
        ResetSelectedFields(); // Initialize SelectedFields
        MakeMoveCommand = new Command(_ =>
        {
            Moves.Add(SelectedFields.Select(x => x.Color));
            ResetSelectedFields();
        });
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

    public ObservableCollection<IEnumerable<string>> Moves { get; } = new();

    public ICommand MakeMoveCommand { get; }

    public string Username
    {
        get => _username;
        set
        {
            if (value == _username || value is null) return;

            PropertyChanging?.Invoke(this, new(nameof(Username)));
            _username = value;
            PropertyChanged?.Invoke(this, new(nameof(Username)));
        }
    }

    private void ResetSelectedFields()
    {
        SelectedFields.Clear();

        for (int i = 0; i < 4; i++)
            SelectedFields.Add(new SelectedField());
    }
}
