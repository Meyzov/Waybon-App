using Waybon.App.Models;

namespace Waybon.App.Services.Interfaces
{
    public interface IGroupService
    {
        Task<IEnumerable<GroupDetails>> GetJoinedGroupsAsync(SessionIdRequest request);
    }
}