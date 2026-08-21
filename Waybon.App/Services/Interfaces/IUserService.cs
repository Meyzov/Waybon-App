using Waybon.App.Models;

namespace Waybon.App.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> UpdateSharingAsync(UpdateSharingRequest request, CancellationToken cancellationToken = default);
        Task<bool> BlockUserAsync(TargetUserRequest request, CancellationToken cancellationToken = default);
        Task<bool> UnblockUserAsync(TargetUserRequest request, CancellationToken cancellationToken = default);
    }
}
