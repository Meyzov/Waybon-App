using System.Net.Http.Json;
using Waybon.App.Models;
using Waybon.App.Services.Interfaces;

namespace Waybon.App.Services.Implementations
{
    public class GroupService(HttpClient httpClient) : IGroupService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<IEnumerable<GroupDetails>> GetJoinedGroupsAsync(SessionIdRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync
            (
                "api/groups/get-joined", request
            );

            if (!response.IsSuccessStatusCode)
            {
                return [];
            }

            return await response.Content.ReadFromJsonAsync<List<GroupDetails>>() ?? [];
        }

        public async Task<IEnumerable<GroupMember>> GetGroupMembersAsync(int groupId, SessionIdRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync
            (
                $"api/groups/{groupId}/get-members", request
            );

            if (!response.IsSuccessStatusCode)
            {
                return [];
            }

            return await response.Content.ReadFromJsonAsync<List<GroupMember>>() ?? [];
        }
    }
}