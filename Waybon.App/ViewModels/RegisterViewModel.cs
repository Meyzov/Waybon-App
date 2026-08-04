using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Waybon.App.Models;
using Waybon.App.Services.Interfaces;

namespace Waybon.App.ViewModels
{
    public partial class RegisterViewModel(IAuthService authService, INavigationService navigationService, IDialogService dialogService) : ObservableObject
    {
        private readonly IAuthService _authService = authService;
        private readonly INavigationService _navigationService = navigationService;
        private readonly IDialogService _dialogService = dialogService;

        [ObservableProperty]
        public partial string Username { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Email { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Password { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string RoleName { get; set; } = string.Empty;

        [ObservableProperty]
        public partial bool IsPassword { get; set; } = true;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RegisterButtonText))]
        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        public partial bool IsBusy { get; set; }

        public string RegisterButtonText
        {
            get
            {
                if (IsBusy)
                {
                    return "Registrando...";
                }

                return "Registrarse";
            }
        }

        [RelayCommand(CanExecute = nameof(CanRegister))]
        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(RoleName))
            {
                await _dialogService.ShowAlertAsync("Error", "Completa todos los campos.", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                var request = new RegisterRequest
                {
                    Username = Username.Trim(),
                    Email = Email.Trim().ToLower(),
                    Password = Password,
                    RoleName = RoleName.Trim()
                };

                var success = await _authService.RegisterAsync(request);
                if (!success)
                {
                    await _dialogService.ShowAlertAsync("Error", "No se pudo completar el registro. Inténtalo de nuevo.", "OK");
                    return;
                }

                await _dialogService.ShowAlertAsync("Éxito", "¡Registro exitoso! Ahora puedes iniciar sesión.", "OK");
                await _navigationService.GoToAsync("//login");
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

        private bool CanRegister()
        {
            if (!IsBusy)
            {
                return true;
            }

            return false;
        }

        [RelayCommand]
        private void TogglePassword()
        {
            IsPassword = !IsPassword;
        }

        [RelayCommand]
        private async Task NavigateToLoginAsync()
        {
            await _navigationService.GoToAsync("//login");
        }
    }
}