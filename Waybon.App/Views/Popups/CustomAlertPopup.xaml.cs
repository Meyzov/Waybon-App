using CommunityToolkit.Maui.Views;

namespace Waybon.App.Views.Popups;

public partial class CustomAlertPopup : Popup
{
    private readonly TaskCompletionSource<bool> _tcs = new();

    public CustomAlertPopup(string title, string message, string cancel, string? accept = null)
    {
        InitializeComponent();

        TitleLabel.Text = title;
        MessageLabel.Text = message;

        if (accept == null)
        {
            CancelButton.IsVisible = false;
            AcceptButton.Text = cancel;

            ButtonsGrid.ColumnDefinitions.Clear();
            ButtonsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            AcceptButton.SetValue(Grid.ColumnProperty, 0);
        }
        else
        {
            CancelButton.Text = cancel;
            AcceptButton.Text = accept;
        }
    }

    public Task<bool> ResultAsync() => _tcs.Task;

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        _tcs.TrySetResult(false);
        await CloseAsync();
    }

    private async void OnAcceptClicked(object sender, EventArgs e)
    {
        _tcs.TrySetResult(true);
        await CloseAsync();
    }
}