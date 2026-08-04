using System.Net.Http.Json;
using Waybon.App.Models;
using Waybon.App.Services.Interfaces;

namespace Waybon.App.Services.Implementations
{
    public class AuthService(HttpClient httpClient) : IAuthService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync
            (
                "api/auth/login", request
            );

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync
            (
                "api/auth/register", request
            );

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<SuccessResponse>();
            if (result == null)
            {
                return false;
            }

            return result.Success;
        }
    }
}
