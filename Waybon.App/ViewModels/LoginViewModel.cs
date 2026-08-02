using Waybon.App.Models;
using Waybon.App.Services.Interfaces;

namespace Waybon.App.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;
        private readonly IPreferencesService _preferencesService;
        private readonly IDialogService _dialogService;

        private string _email = string.Empty;
        private string _password = string.Empty;
        private bool _isPassword = true;
        private bool _isBusy;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public bool IsPassword
        {
            get => _isPassword;
            set => SetProperty(ref _isPassword, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    LoginCommand.ChangeCanExecute();
                }
            }
        }

        public Command LoginCommand { get; }
        public Command TogglePasswordCommand { get; }
        public Command NavigateToRegisterCommand { get; }
        public Command ForgotPasswordCommand { get; }

        public LoginViewModel(IAuthService authService, INavigationService navigationService, IPreferencesService preferencesService, IDialogService dialogService)
        {
            _authService = authService;
            _navigationService = navigationService;
            _preferencesService = preferencesService;
            _dialogService = dialogService;

            LoginCommand = new Command(async () => await LoginAsync(), () => !IsBusy);
            TogglePasswordCommand = new Command(() => IsPassword = !IsPassword);
            NavigateToRegisterCommand = new Command(async () => await _navigationService.GoToAsync("//register"));
            ForgotPasswordCommand = new Command(async () => await _dialogService.ShowAlertAsync("Recuperar contraseña", "Función no implementada.", "OK"));
        }

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
                var result = await _authService.LoginAsync(Email.Trim(), Password);

                if (result is null)
                {
                    await _dialogService.ShowAlertAsync("Error", "Credenciales inválidas.", "OK");
                    return;
                }

                SaveSession(result);
                await _navigationService.GoToAsync("//profile");
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
