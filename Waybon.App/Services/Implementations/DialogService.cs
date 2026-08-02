using Waybon.App.Services.Interfaces;

namespace Waybon.App.Services.Implementations
{
    public class DialogService : IDialogService
    {
        public Task ShowAlertAsync(string title, string message, string cancel)
        {
            return Shell.Current.CurrentPage.DisplayAlertAsync(title, message, cancel);
        }
    }
}