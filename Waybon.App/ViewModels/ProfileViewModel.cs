using Waybon.App.Services.Interfaces;

namespace Waybon.App.ViewModels
{
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IPreferencesService _preferencesService;

        private string _username = string.Empty;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public Command LogoutCommand { get; }

        public ProfileViewModel(INavigationService navigationService, IPreferencesService preferencesService)
        {
            _navigationService = navigationService;
            _preferencesService = preferencesService;

            Username = _preferencesService.Get("waybon_username", "Usuario");
            LogoutCommand = new Command(async () => await LogoutAsync());
        }

        private async Task LogoutAsync()
        {
            _preferencesService.Clear();
            await _navigationService.GoToAsync("//login");
        }
    }
}
