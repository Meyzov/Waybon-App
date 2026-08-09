using Waybon.App.Models;

namespace Waybon.App.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> UpdateSharingAsync(UpdateSharingRequest request, CancellationToken cancellationToken = default);
    }
}
