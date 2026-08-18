using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Waybon.App.Models
{
    public partial class GroupMember : ObservableObject
    {
        [ObservableProperty]
        public partial Guid UserId { get; set; }

        [ObservableProperty]
        public partial string Username { get; set; } = string.Empty;

        [ObservableProperty]
        public partial bool SharingEnabled { get; set; }

        [ObservableProperty]
        public partial bool BlockedByMe { get; set; }

        [ObservableProperty]
        public partial bool BlockingMe { get; set; }

        [ObservableProperty]
        public partial DateTime? LastActivityAt { get; set; }

        [JsonIgnore]
        [ObservableProperty]
        public partial string DisplayUsername { get; set; } = string.Empty;

        [JsonIgnore]
        [ObservableProperty]
        public partial ObservableCollection<string> Tags { get; set; } = [];
    }
}