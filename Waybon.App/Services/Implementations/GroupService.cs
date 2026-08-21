using System.Net.Http.Json;
using Waybon.App.Models;
using Waybon.App.Services.Interfaces;

namespace Waybon.App.Services.Implementations
{
    public class GroupService(HttpClient httpClient) : IGroupService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<IEnumerable<GroupDetails>> GetJoinedGroupsAsync(SessionIdRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync
            (
                "api/groups/get-joined", request, cancellationToken
            );

            if (!response.IsSuccessStatusCode)
            {
                return [];
            }

            return await response.Content.ReadFromJsonAsync<List<GroupDetails>>(cancellationToken) ?? [];
        }

        public async Task<IEnumerable<GroupMember>> GetGroupMembersAsync(int groupId, SessionIdRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync
            (
                $"api/groups/{groupId}/get-members", request, cancellationToken
            );

            if (!response.IsSuccessStatusCode)
            {
                return [];
            }

            return await response.Content.ReadFromJsonAsync<List<GroupMember>>(cancellationToken) ?? [];
        }

        public async Task<bool> CreateGroupAsync(CreateGroupRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync
            (
                "api/groups/create", request, cancellationToken
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

        public async Task<bool> JoinGroupAsync(JoinGroupRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync
            (
                "api/groups/join", request, cancellationToken
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

        public async Task<RegenerateJoinCodeResponse?> RegenerateJoinCodeAsync(int groupId, SessionIdRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync
            (
                $"api/groups/{groupId}/regenerate-code", request, cancellationToken
            );

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<RegenerateJoinCodeResponse>(cancellationToken);
        }

        public async Task<bool> DeleteGroupAsync(int groupId, SessionIdRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync
            (
                $"api/groups/{groupId}/delete", request, cancellationToken
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

        public async Task<bool> LeaveGroupAsync(int groupId, SessionIdRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync
            (
                $"api/groups/{groupId}/leave", request, cancellationToken
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

        public async Task<bool> KickMemberAsync(int groupId, TargetUserRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync
            (
                $"api/groups/{groupId}/kick-member", request, cancellationToken
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