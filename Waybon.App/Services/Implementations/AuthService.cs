using System.Net.Http.Json;
using Waybon.App.Models;
using Waybon.App.Services.Interfaces;

namespace Waybon.App.Services.Implementations
{
    public class AuthService(HttpClient httpClient) : IAuthService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync
            (
                "api/auth/login", new
                {
                    email, password
                }
            );

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }
    }
}
