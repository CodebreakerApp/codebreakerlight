using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace CodeBreaker.ViewModels.Services;

public class InfoBarMessageService
{
    public ObservableCollection<InfoMessageViewModel> Messages { get; } = new();

    public void ShowMessage(InfoMessageViewModel message)
    {
        message.ContainingCollection = Messages;
        Messages.Add(message);
    }

    public void ShowInformation(string content) => ShowMessage(InfoMessageViewModel.Information(content));

    public void ShowWarning(string content) => ShowMessage(InfoMessageViewModel.Warning(content));

    public void ShowError(string content) => ShowMessage(InfoMessageViewModel.Error(content));

    public void ShowSuccess(string content) => ShowMessage(InfoMessageViewModel.Success(content));

    public void Clear() =>
        Messages.Clear();
}
