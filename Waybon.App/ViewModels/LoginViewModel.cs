using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Waybon.App.Models;
using Waybon.App.Services.Interfaces;

namespace Waybon.App.ViewModels
{
    public partial class LoginViewModel(IAuthService authService, INavigationService navigationService, IPreferencesService preferencesService, IDialogService dialogService) : ObservableObject
    {
        private readonly IAuthService _authService = authService;
        private readonly INavigationService _navigationService = navigationService;
        private readonly IPreferencesService _preferencesService = preferencesService;
        private readonly IDialogService _dialogService = dialogService;


        // ======================
        // Properties
        // ======================

        [ObservableProperty]
        public partial string Email { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Password { get; set; } = string.Empty;

        [ObservableProperty]
        public partial bool IsPassword { get; set; } = true;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(LoginButtonText))]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        public partial bool IsBusy { get; set; }

        public string LoginButtonText => IsBusy ? "Entrando..." : "Entrar";

        // ======================
        // Commands
        // ======================

        [RelayCommand(CanExecute = nameof(CanLogin))]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await _dialogService.ShowAlertAsync("Error", "Completa todos los campos.", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                var request = new LoginRequest
                {
                    Email = Email.Trim().ToLower(),
                    Password = Password,
                };

                var result = await _authService.LoginAsync(request);
                if (result == null)
                {
                    await _dialogService.ShowAlertAsync("Error", "Credenciales inválidas.", "OK");
                    return;
                }

                SaveSession(result);
                await _navigationService.GoToAsync("//main");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync("Error", $"No se pudo conectar: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void TogglePassword()
        {
            IsPassword = !IsPassword;
        }

        [RelayCommand]
        private async Task NavigateToRegisterAsync()
        {
            await _navigationService.GoToAsync("//register");
        }

        [RelayCommand]
        private async Task ForgotPasswordAsync()
        {
            await _dialogService.ShowAlertAsync("Recuperar contraseña", "Función no implementada.", "OK");
        }


        // ======================
        // Helpers
        // ======================

        private bool CanLogin() => !IsBusy;

        private void SaveSession(LoginResponse result)
        {
            _preferencesService.Set("waybon_sessionId", result.SessionId.ToString());
            _preferencesService.Set("waybon_userId", result.UserId.ToString());
            _preferencesService.Set("waybon_username", result.Username);
            _preferencesService.Set("waybon_roleName", result.RoleName);
            _preferencesService.Set("waybon_sharingEnabled", result.SharingEnabled.ToString());
        }
    }
}