namespace Codebreaker.MAUI.Views.Pages;

public partial class GamePage : ContentPage
{

	public GamePage(GamePageViewModel viewModel)
	{
		ViewModel = viewModel;

		BindingContext = this;
		InitializeComponent();

		WeakReferenceMessenger.Default.Register<InfoMessage>(this, async (r, m) =>
		{
			await DisplayAlert("Info", m.Text, "Close");
		});
	}

	public GamePageViewModel ViewModel { get; }
}
