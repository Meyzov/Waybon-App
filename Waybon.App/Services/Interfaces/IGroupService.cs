using Waybon.App.Models;

namespace Waybon.App.Services.Interfaces
{
    public interface IGroupService
    {
        Task<IEnumerable<GroupDetails>> GetJoinedGroupsAsync(SessionIdRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<GroupMember>> GetGroupMembersAsync(int groupId, SessionIdRequest request, CancellationToken cancellationToken = default);
        Task<bool> CreateGroupAsync(CreateGroupRequest request, CancellationToken cancellationToken = default);
        Task<bool> JoinGroupAsync(JoinGroupRequest request, CancellationToken cancellationToken = default);
        Task<RegenerateJoinCodeResponse?> RegenerateJoinCodeAsync(int groupId, SessionIdRequest request, CancellationToken cancellationToken = default);
    }
}