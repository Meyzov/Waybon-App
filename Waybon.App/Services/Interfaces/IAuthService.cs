using Waybon.App.Models;

namespace Waybon.App.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(string email, string password);
    }
}