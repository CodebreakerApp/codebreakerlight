using System.ComponentModel;

namespace MauiMvvmDemo.ViewModels.Models;

public class SelectedField : INotifyPropertyChanging, INotifyPropertyChanged
{
    private string _color = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public event PropertyChangingEventHandler? PropertyChanging;

    public SelectedField()
    {
    }

    public SelectedField(string color)
    {
        Color = color;
    }

    public string Color
    {
        get => _color;
        set
        {
            if (value == _color || value is null)
                return;

            PropertyChanging?.Invoke(this, new (nameof(Color)));
            _color = value;
            PropertyChanged?.Invoke(this, new (nameof(Color)));
        }
    }

    public static implicit operator SelectedField(string color) => new SelectedField(color);
    public static implicit operator string(SelectedField value) => value.Color;
}
