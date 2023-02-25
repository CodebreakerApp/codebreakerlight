using CodeBreaker.ViewModels.Services;

using CommunityToolkit.Mvvm.Messaging;

namespace CodeBreaker.MAUI.Services;

public class MauiDialogService : IDialogService
{
    public Task ShowMessageAsync(string message)
    {
        WeakReferenceMessenger.Default.Send(new InfoMessage(message));
        return Task.CompletedTask;
    }
}

public record InfoMessage(string Text);
