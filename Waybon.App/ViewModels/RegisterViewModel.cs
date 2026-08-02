using Waybon.App.Models;
using Waybon.App.Services.Interfaces;

namespace Waybon.App.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        private string _username = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _roleName = string.Empty;
        private bool _isPassword = true;
        private bool _isBusy;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

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

        public string RoleName
        {
            get => _roleName;
            set => SetProperty(ref _roleName, value);
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
                    RegisterCommand.ChangeCanExecute();
                }
            }
        }

        public Command RegisterCommand { get; }
        public Command TogglePasswordCommand { get; }
        public Command NavigateToLoginCommand { get; }

        public RegisterViewModel(IAuthService authService, INavigationService navigationService, IDialogService dialogService)
        {
            _authService = authService;
            _navigationService = navigationService;
            _dialogService = dialogService;

            RegisterCommand = new Command(async () => await RegisterAsync(), () => !IsBusy);
            TogglePasswordCommand = new Command(() => IsPassword = !IsPassword);
            NavigateToLoginCommand = new Command(async () => await _navigationService.GoToAsync("//login"));
        }

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
    }
}