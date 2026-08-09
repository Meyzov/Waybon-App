using System;
using System.Collections.Generic;
using System.Text;
using Waybon.App.Data.Entities;

namespace Waybon.App.Data.Interfaces
{
    public interface ILocationRepository
    {
        Task SaveLocationAsync(UserLocation location);
        Task<UserLocation> GetLocationAsync(Guid userId);
        Task<List<UserLocation>> GetLocationsAsync(IEnumerable<Guid> userIds);
        Task ClearAllLocationsAsync();
    }
}
