using System.Net.Http.Json;
using Waybon.App.Models;
using Waybon.App.Services.Interfaces;

namespace Waybon.App.Services.Implementations
{
    public class UserService(HttpClient httpClient) : IUserService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<bool> UpdateSharingAsync(UpdateSharingRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync
            (
                "api/user/sharing", request, cancellationToken
            );

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<SuccessResponse>(cancellationToken);
            if (result == null)
            {
                return false;
            }

            return result.Success;
        }

        public async Task<bool> BlockUserAsync(TargetUserRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync
            (
                "api/user/block", request, cancellationToken
            );

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<SuccessResponse>(cancellationToken);
            if (result == null)
            {
                return false;
            }

            return result.Success;
        }

        public async Task<bool> UnblockUserAsync(TargetUserRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync
            (
                "api/user/unblock", request, cancellationToken
            );

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<SuccessResponse>(cancellationToken);
            if (result == null)
            {
                return false;
            }

            return result.Success;
        }
    }
}
