using Waybon.App.Models;

namespace Waybon.App.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<bool> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    }
}