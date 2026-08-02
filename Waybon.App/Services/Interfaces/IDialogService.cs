namespace Waybon.App.Services.Interfaces
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string title, string message, string cancel);
    }
}