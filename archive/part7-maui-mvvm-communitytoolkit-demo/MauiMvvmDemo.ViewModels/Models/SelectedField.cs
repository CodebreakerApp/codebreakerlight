using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiMvvmDemo.ViewModels.Models;

public partial class SelectedField : ObservableObject
{
    [ObservableProperty]
    private string _color = string.Empty;

    public SelectedField()
    {
    }

    public SelectedField(string color)
    {
        Color = color;
    }

    public static implicit operator SelectedField(string color) => new SelectedField(color);
    public static implicit operator string(SelectedField value) => value.Color;
}
