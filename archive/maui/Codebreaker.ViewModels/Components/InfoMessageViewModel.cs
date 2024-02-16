using System.Windows.Input;

namespace Codebreaker.ViewModels;

public enum InfoMessageSeverity
{
    Info,
    Success,
    Warning,
    Error
}

public partial class InfoMessageViewModel : ObservableObject
{
    public static InfoMessageViewModel Error(string content)
    {
        InfoMessageViewModel message = new()
        {
            Title = "Error",
            Message = content,
            Severity = InfoMessageSeverity.Error,
            ActionTitle = "OK"
        };
        message.ActionCommand = new RelayCommand(() => message.Close());
        return message;
    }

    public static InfoMessageViewModel Warning(string content)
    {
        InfoMessageViewModel message = new()
        {
            Title = "Warning",
            Message = content,
            Severity = InfoMessageSeverity.Warning,
            ActionTitle = "OK"
        };
        message.ActionCommand = new RelayCommand(() => message.Close());
        return message;
    }

    public static InfoMessageViewModel Information(string content)
    {
        InfoMessageViewModel message = new()
        {
            Title = "Information",
            Message = content,
            Severity = InfoMessageSeverity.Info,
            ActionTitle = "OK"
        };
        message.ActionCommand = new RelayCommand(() => message.Close());
        return message;
    }

    public static InfoMessageViewModel Success(string content)
    {
        InfoMessageViewModel message = new()
        {
            Title = "Success",
            Message = content,
            Severity = InfoMessageSeverity.Success,
            ActionTitle = "OK"
        };
        message.ActionCommand = new RelayCommand(() => message.Close());
        return message;
    }

    internal ICollection<InfoMessageViewModel>? ContainingCollection { get; set; }

    [ObservableProperty]
    private InfoMessageSeverity _severity = InfoMessageSeverity.Info;

    [ObservableProperty]
    private string _message = string.Empty;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasAction))]
    private ICommand? _actionCommand;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasAction))]
    private string? _actionTitle = "OK";

    public bool HasAction =>
        ActionCommand is not null && ActionTitle is not null;

    public void Close() =>
        ContainingCollection?.Remove(this);
}
