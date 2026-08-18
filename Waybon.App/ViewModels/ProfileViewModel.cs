using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Waybon.App.Data.Interfaces;
using Waybon.App.Data.Repositories;
using Waybon.App.Models;
using Waybon.App.Services.Implementations;
using Waybon.App.Services.Interfaces;

namespace Waybon.App.ViewModels
{
    public partial class ProfileViewModel(INavigationService navigationService, IPreferencesService preferencesService, ISessionService sessionService, IUserService userService, IDatabaseService databaseService) : ObservableObject
    {
        private readonly INavigationService _navigationService = navigationService;
        private readonly IPreferencesService _preferencesService = preferencesService;
        private readonly ISessionService _sessionService = sessionService;
        private readonly IUserService _userService = userService;
        private readonly IDatabaseService _databaseService = databaseService;

        private const string UsernameKey = "waybon_username";
        private const string RoleNameKey = "waybon_roleName";
        private const string SharingEnabledKey = "waybon_sharingEnabled";


        // ======================
        // Properties
        // ======================

        [ObservableProperty]
        public partial string Username { get; set; } = preferencesService.Get(UsernameKey, "Usuario");

        [ObservableProperty]
        public partial string RoleName { get; set; } = preferencesService.Get(RoleNameKey, "Rol");

        [ObservableProperty]
        public partial bool IsSharingEnabled { get; set; } = bool.TryParse(preferencesService.Get(SharingEnabledKey, "false"), out var enabled) && enabled;


        // ======================
        // Cancellation Sources
        // ======================

        private CancellationTokenSource? _sharingLoadCts;


        // ======================
        // Commands
        // ======================

        [RelayCommand]
        private async Task LogoutAsync()
        {
            _sessionService.ClearSession();
            await _databaseService.ClearAllAsync();
            await _navigationService.GoToAsync("//login");
        }

        [RelayCommand]
        private async Task ToggleSharingAsync()
        {
            _ = ExecuteToggleSharingAsync(!IsSharingEnabled);
        }

        private async Task ExecuteToggleSharingAsync(bool targetState)
        {
            _sharingLoadCts?.Cancel();

            var cts = new CancellationTokenSource();
            _sharingLoadCts = cts;

            try
            {
                if (_sessionService.SessionId == Guid.Empty)
                {
                    return;
                }

                var request = new UpdateSharingRequest
                {
                    SessionId = _sessionService.SessionId,
                    SharingEnabled = targetState
                };

                IsSharingEnabled = targetState;
                var sharingUpdated = await _userService.UpdateSharingAsync(request, cts.Token);

                // ======================
                // Abort if stale
                // ======================

                cts.Token.ThrowIfCancellationRequested();

                if (sharingUpdated)
                {
                    IsSharingEnabled = targetState;
                    _preferencesService.Set(SharingEnabledKey, targetState.ToString());
                }
                else
                {
                    IsSharingEnabled = !targetState;
                }
            }
            catch (OperationCanceledException)
            {
                // Ignore
            }
            catch (Exception ex)
            {
                if (_sharingLoadCts == cts)
                {
                    IsSharingEnabled = !targetState;
                }

                System.Diagnostics.Debug.WriteLine($"Error updating location sharing: {ex.Message}");
            }
            finally
            {
                if (_sharingLoadCts == cts)
                {
                    _sharingLoadCts = null;
                }

                cts.Dispose();
            }
        }
    }
}