using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace Waybon.App.Models
{
    public partial class GroupDetails : ObservableObject
    {
        [ObservableProperty]
        public partial int GroupId { get; set; }

        [ObservableProperty]
        public partial Guid OwnerUserId { get; set; }

        [ObservableProperty]
        public partial string Username { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Name { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string? JoinCode { get; set; }

        [ObservableProperty]
        public partial DateTime? JoinCodeExpiresAt { get; set; }

        [ObservableProperty]
        public partial DateTime CreatedAt { get; set; }

        [JsonIgnore]
        [ObservableProperty]
        public partial string DisplayUsername { get; set; } = string.Empty;
    }
}