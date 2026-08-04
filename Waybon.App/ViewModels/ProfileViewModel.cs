using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Waybon.App.Services.Interfaces;

namespace Waybon.App.ViewModels
{
    public partial class ProfileViewModel(INavigationService navigationService, IPreferencesService preferencesService, ISessionService sessionService) : ObservableObject
    {
        private readonly INavigationService _navigationService = navigationService;
        private readonly ISessionService _sessionService = sessionService;

        [ObservableProperty]
        public partial string Username { get; set; } = preferencesService.Get("waybon_username", "Usuario");

        [ObservableProperty]
        public partial string RoleName { get; set; } = preferencesService.Get("waybon_roleName", "Rol");

        [RelayCommand]
        private async Task LogoutAsync()
        {
            _sessionService.ClearSession();
            await _navigationService.GoToAsync("//login");
        }
    }
}