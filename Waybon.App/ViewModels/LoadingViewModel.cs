using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Waybon.App.Data.Interfaces;
using Waybon.App.Services.Interfaces;

namespace Waybon.App.ViewModels
{
    public partial class LoadingViewModel(IDatabaseService databaseService, ISessionService sessionService, INavigationService navigationService) : ObservableObject
    {
        private readonly IDatabaseService _databaseService = databaseService;
        private readonly ISessionService _sessionService = sessionService;
        private readonly INavigationService _navigationService = navigationService;


        // ======================
        // Commands
        // ======================

        [RelayCommand]
        public async Task InitializeAsync()
        {
            await _databaseService.InitializeAsync();

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