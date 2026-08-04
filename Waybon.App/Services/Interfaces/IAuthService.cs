using Waybon.App.Models;

namespace Waybon.App.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
        Task<bool> RegisterAsync(RegisterRequest request);
    }
}