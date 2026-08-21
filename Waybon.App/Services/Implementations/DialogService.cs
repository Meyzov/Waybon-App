using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using Waybon.App.Services.Interfaces;
using Waybon.App.Views.Popups;

namespace Waybon.App.Services.Implementations;

public class DialogService : IDialogService
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task ShowAlertAsync(string title, string message, string cancel)
    {
        await _semaphore.WaitAsync();
        try
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
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> ShowConfirmAsync(string title, string message, string accept, string cancel)
    {
        await _semaphore.WaitAsync();
        try
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
        finally
        {
            _semaphore.Release();
        }
    }
}