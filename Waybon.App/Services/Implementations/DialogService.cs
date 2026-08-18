using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Waybon.App.Services.Interfaces;
using Waybon.App.Views.Popups;

namespace Waybon.App.Services.Implementations;

public class DialogService : IDialogService
{
    public async Task ShowAlertAsync(string title, string message, string cancel)
    {
        var page = Shell.Current.CurrentPage;
        var popup = new CustomAlertPopup(title, message, cancel);

        await page.ShowPopupAsync
        (
            popup,
            new PopupOptions
            {
                Shape = null,
                Shadow = null,
                CanBeDismissedByTappingOutsideOfPopup = false
            }
        );

        await popup.ResultAsync();
    }

    public async Task<bool> ShowConfirmAsync(string title, string message, string accept, string cancel)
    {
        var page = Shell.Current.CurrentPage;
        var popup = new CustomAlertPopup(title, message, cancel, accept);

        await page.ShowPopupAsync
        (
            popup,
            new PopupOptions
            {
                Shape = null,
                Shadow = null,
                CanBeDismissedByTappingOutsideOfPopup = false
            }
        );

        return await popup.ResultAsync();
    }
}