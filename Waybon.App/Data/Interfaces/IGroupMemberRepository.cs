using Waybon.App.Data.Entities;

namespace Waybon.App.Data.Interfaces
{
    public interface IGroupMemberRepository
    {
        Task SaveUserAsync(LocalUser user);

        Task SaveMembersAsync(int groupId, IEnumerable<LocalUser> users);

        Task<IEnumerable<LocalUser>> GetMembersAsync(int groupId);

        Task ClearMembersAsync(int groupId);

        Task ClearAllMembersAsync();
    }
}