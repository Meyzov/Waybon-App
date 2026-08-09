using Waybon.App.Data.Entities;

namespace Waybon.App.Data.Interfaces
{
    public interface IGroupRepository
    {
        Task SaveGroupsAsync(IEnumerable<LocalGroup> groups);
        Task<List<LocalGroup>> GetGroupsAsync();
        Task ClearAllGroupsAsync();
    }
}