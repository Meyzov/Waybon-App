using Waybon.App.Services.Implementations;
using Waybon.App.Services.Interfaces;

namespace Waybon.App.ViewModels
{
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IPreferencesService _preferencesService;
        private readonly ISessionService _sessionService;

        private string _username = string.Empty;
        private string _roleName = string.Empty;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string RoleName
        {
            get => _roleName;
            set => SetProperty(ref _roleName, value);
        }

        public Command LogoutCommand { get; }

        public ProfileViewModel(INavigationService navigationService, IPreferencesService preferencesService, ISessionService sessionService)
        {
            _navigationService = navigationService;
            _preferencesService = preferencesService;
            _sessionService = sessionService;

            Username = _preferencesService.Get("waybon_username", "Usuario");
            RoleName = _preferencesService.Get("waybon_roleName", "Rol");
            LogoutCommand = new Command(async () => await LogoutAsync());
        }

        private async Task LogoutAsync()
        {
            _sessionService.ClearSession();
            await _navigationService.GoToAsync("//login");
        }
    }
}