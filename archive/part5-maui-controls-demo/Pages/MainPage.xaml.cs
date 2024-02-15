using System.Collections.ObjectModel;

namespace MauiControlsDemo.Pages;

public partial class MainPage : ContentPage
{
    private string _selectedColor = string.Empty;

    private readonly ObservableCollection<string> _nameHistory = new();

    public MainPage()
    {
        BindingContext = this;
        InitializeComponent();
        nameHistoryListView.ItemsSource = _nameHistory;
    }

    public string[] AvailableColors { get; } =
    [
        "Red",
        "Blue",
        "Green",
        "Yellow",
        "Orange",
        "White",
        "Black"
    ];

    public string SelectedColor
    {
        get => _selectedColor;
        set
        {
            if (value is null)
                return;

            _selectedColor = value;
            Circles.Add(value);
        }
    }

    public ICollection<string> Circles { get; } = new ObservableCollection<string>();

    private void OnSubmitButtonClicked(object sender, EventArgs e)
    {
        string nameInput = nameEntry.Text;
        _ = DisplayAlert("Hello", $"Your name is {nameInput}.", "Ok thank you");
        
        _nameHistory.Add(nameInput);
        nameHistoryListView.IsVisible = true;
    }

    private async void OnToAboutClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///AboutPage");
    }
}