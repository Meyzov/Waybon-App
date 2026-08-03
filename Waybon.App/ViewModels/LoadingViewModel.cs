using Waybon.App.Services.Interfaces;

namespace Waybon.App.ViewModels
{
    public class LoadingViewModel(ISessionService sessionService, INavigationService navigationService) : BaseViewModel
    {
        private readonly ISessionService _sessionService = sessionService;
        private readonly INavigationService _navigationService = navigationService;

        public async Task InitializeAsync()
        {
            if (_sessionService.IsAuthenticated)
            {
                await _navigationService.GoToAsync("//main");
            }
            else
            {
                await _navigationService.GoToAsync("//login");
            }
        }
    }
}