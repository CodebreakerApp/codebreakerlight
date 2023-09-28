namespace CodeBreaker.Services;

public interface IViewModelNavigationService
{
    bool NavigateToViewModel(string viewModelKey, object? parameter = null, bool clearNavigation = false);

    bool NavigateToViewModel(Type viewModelType, object? parameter = default, bool clearNavigation = false) =>
        NavigateToViewModel(viewModelType?.FullName ?? throw new ArgumentNullException(nameof(viewModelType)), parameter, clearNavigation);

    bool GoBack();
}
