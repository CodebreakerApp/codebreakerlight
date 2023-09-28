using MauiMvvmDemo.MVVM.Pages;

namespace MauiMvvmDemo.Pages;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageVM vm)
    {
        VM = vm;
        BindingContext = this;
        InitializeComponent();
    }

    public MainPageVM VM { get; }
}

