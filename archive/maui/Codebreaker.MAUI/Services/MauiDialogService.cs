namespace Codebreaker.MAUI.Services;

public class MauiDialogService : IDialogService
{
    public Task ShowMessageAsync(string message)
    {
        WeakReferenceMessenger.Default.Send(new InfoMessage(message));
        return Task.CompletedTask;
    }
}

public record InfoMessage(string Text);
