using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Waybon.App.Services.Interfaces;

namespace Waybon.App.ViewModels
{
    public partial class LoadingViewModel(ISessionService sessionService, INavigationService navigationService) : ObservableObject
    {
        private readonly ISessionService _sessionService = sessionService;
        private readonly INavigationService _navigationService = navigationService;

        [RelayCommand]
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