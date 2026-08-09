using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Waybon.App.Data.Interfaces;
using Waybon.App.Services.Interfaces;

namespace Waybon.App.ViewModels
{
    public partial class ProfileViewModel(INavigationService navigationService, IPreferencesService preferencesService, ISessionService sessionService, IDatabaseService databaseService) : ObservableObject
    {
        private readonly INavigationService _navigationService = navigationService;
        private readonly ISessionService _sessionService = sessionService;
        private readonly IDatabaseService _databaseService = databaseService;


        // ======================
        // Properties
        // ======================

        [ObservableProperty]
        public partial string Username { get; set; } = preferencesService.Get("waybon_username", "Usuario");

        [ObservableProperty]
        public partial string RoleName { get; set; } = preferencesService.Get("waybon_roleName", "Rol");


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
    }
}