using System.Windows.Input;

namespace MauiMvvmDemo.MVVM.Commands;

public class Command : ICommand
{
    private readonly Action<object?> _action;

    private readonly Func<object?, bool>? _canExecute;

    public event EventHandler? CanExecuteChanged;

    public Command(Action<object?> action)
    {
        _action = action;
    }

    public Command(Action<object?> action, Func<object?, bool> canExecute)
    {
        _action = action;
        _canExecute = canExecute;

    }

    public bool CanExecute(object? parameter) =>
        _canExecute?.Invoke(parameter) ?? true;

    public void Execute(object? parameter) =>
        _action?.Invoke(parameter);
}
