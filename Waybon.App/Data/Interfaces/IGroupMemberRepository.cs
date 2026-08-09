using Waybon.App.Data.Entities;

namespace Waybon.App.Data.Interfaces
{
    public interface IGroupMemberRepository
    {
        Task SaveMembersAsync(int groupId, IEnumerable<LocalMember> members);
        Task<List<LocalMember>> GetMembersAsync(int groupId);
        Task ClearMembersAsync(int groupId);
        Task ClearAllMembersAsync();
    }
}