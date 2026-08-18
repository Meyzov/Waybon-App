using Waybon.App.Data.Entities;

namespace Waybon.App.Data.Interfaces
{
    public interface IGroupRepository
    {
        Task SaveGroupsAsync(IEnumerable<LocalGroup> groups);
        Task<IEnumerable<LocalGroup>> GetGroupsAsync();
        Task<LocalGroup?> GetGroupByIdAsync(int groupId);
        Task SaveGroupAsync(LocalGroup group);
        Task ClearAllGroupsAsync();
    }
}