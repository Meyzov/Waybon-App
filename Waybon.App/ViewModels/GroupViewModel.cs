using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;
using Waybon.App.Models;
using Waybon.App.Services.Interfaces;

namespace Waybon.App.ViewModels
{
    public partial class GroupViewModel(IGroupService groupService, IPreferencesService preferencesService) : ObservableObject
    {
        private const string GroupsKey = "waybon_groups";
        private const string SelectedGroupIdKey = "waybon_selectedGroupId";
        private const string SelectedGroupNameKey = "waybon_selectedGroupName";
        private const string SelectedGroupOwnerIdKey = "waybon_selectedGroupOwnerId";
        private const string SessionIdKey = "waybon_sessionId";
        private const string UserIdKey = "waybon_userId";

        // --- Groups ---

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowGroups))]
        [NotifyPropertyChangedFor(nameof(ShowIsGroupsEmpty))]
        [NotifyPropertyChangedFor(nameof(ShowIsLoadingGroups))]
        public partial IEnumerable<GroupDetails> JoinedGroups { get; set; } = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowGroups))]
        [NotifyPropertyChangedFor(nameof(ShowIsGroupsEmpty))]
        [NotifyPropertyChangedFor(nameof(ShowIsLoadingGroups))]
        public partial bool IsGroupsLoading { get; set; }

        private bool HasGroups => JoinedGroups.Any();

        public bool ShowGroups => HasGroups && !IsGroupsLoading && !IsGroupSelected;
        public bool ShowIsGroupsEmpty => !HasGroups && !IsGroupsLoading;
        public bool ShowIsLoadingGroups => !HasGroups && IsGroupsLoading;


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowGroups))]
        public partial bool IsGroupSelected {  get; set; }

        [ObservableProperty]
        public partial string SelectedGroupName { get; set; } = "Mis Grupos";

        public int SelectedGroupId { get; set; } = 0;
        public Guid SelectedGroupOwnerId { get; set; } = Guid.Empty;
        private string GroupMembersCacheKey => $"{GroupsKey}_{SelectedGroupId}";

        // --- Group Members ---

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowMembers))]
        [NotifyPropertyChangedFor(nameof(ShowIsMembersEmpty))]
        [NotifyPropertyChangedFor(nameof(ShowIsLoadingMembers))]
        public partial IEnumerable<GroupMember> GroupMembers { get; set; } = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowMembers))]
        [NotifyPropertyChangedFor(nameof(ShowIsMembersEmpty))]
        [NotifyPropertyChangedFor(nameof(ShowIsLoadingMembers))]
        public partial bool IsMembersLoading { get; set; }

        private bool HasMembers => GroupMembers.Any();

        public bool ShowMembers => HasMembers && !IsMembersLoading;
        public bool ShowIsMembersEmpty => !HasMembers && !IsMembersLoading;
        public bool ShowIsLoadingMembers => !HasMembers && IsMembersLoading;

        public async Task LoadGroupsAsync()
        {
            IsGroupsLoading = true;
            await RestoreLastGroupSelection();

            var groupsText = preferencesService.Get(GroupsKey);
            if (!string.IsNullOrEmpty(groupsText))
            {
                var cachedGroups = JsonSerializer.Deserialize<IEnumerable<GroupDetails>>(groupsText) ?? [];
                Formatroups(cachedGroups);

                JoinedGroups = cachedGroups;
            }

            if (HasGroups)
            {
                IsGroupsLoading = false;
            }

            try
            {
                if (!Guid.TryParse(preferencesService.Get(SessionIdKey), out Guid sessionId))
                {
                    return;
                }

                var request = new SessionIdRequest
                {
                    SessionId = sessionId
                };

                var groups = await groupService.GetJoinedGroupsAsync(request);
                if (groups == null)
                {
                    JoinedGroups = [];
                    preferencesService.Set(GroupsKey, string.Empty);
                    return;
                }
                Formatroups(groups);

                JoinedGroups = groups;
                preferencesService.Set(GroupsKey, JsonSerializer.Serialize(groups));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                IsGroupsLoading = false;

                if (IsGroupSelected && !JoinedGroups.Any(g => g.GroupId == SelectedGroupId))
                {
                    BackToGroups();
                }
            }
        }

        private void Formatroups(IEnumerable<GroupDetails> groups)
        {
            var currentUserId = Guid.Empty;
            if (Guid.TryParse(preferencesService.Get(UserIdKey), out var parsedUserId))
            {
                currentUserId = parsedUserId;
            }

            foreach (var group in groups)
            {
                group.DisplayUsername = group.Username;
                if (group.OwnerUserId == currentUserId)
                {
                    group.DisplayUsername += " (Yo)";
                }
            }
        }

        private async Task RestoreLastGroupSelection()
        {
            var selectedGroupName = preferencesService.Get(SelectedGroupNameKey);
            if (!int.TryParse(preferencesService.Get(SelectedGroupIdKey), out var selectedGroupId) || string.IsNullOrEmpty(selectedGroupName) || !Guid.TryParse(preferencesService.Get(SelectedGroupOwnerIdKey), out var selectedGroupOwnerId))
            {
                IsGroupSelected = false; 
                preferencesService.Set(SelectedGroupIdKey, string.Empty);
                preferencesService.Set(SelectedGroupNameKey, string.Empty);
                preferencesService.Set(SelectedGroupOwnerIdKey, string.Empty);
                return;
            }

            IsGroupSelected = true;
            SelectedGroupName = selectedGroupName;
            SelectedGroupId = selectedGroupId;
            SelectedGroupOwnerId = selectedGroupOwnerId;

            await LoadGroupMembersAsync();
        }

        private async Task LoadGroupMembersAsync()
        {
            IsMembersLoading = true;

            var groupMembersText = preferencesService.Get(GroupMembersCacheKey);
            if (!string.IsNullOrEmpty(groupMembersText))
            {
                var cachedMembers = JsonSerializer.Deserialize<IEnumerable<GroupMember>>(groupMembersText) ?? [];
                FormatMembers(cachedMembers);

                GroupMembers = cachedMembers;
            }

            if (HasMembers)
            {
                IsMembersLoading = false;
            }

            try
            {
                if (!Guid.TryParse(preferencesService.Get(SessionIdKey), out Guid sessionId))
                {
                    return;
                }

                var request = new SessionIdRequest
                {
                    SessionId = sessionId
                };

                var members = await groupService.GetGroupMembersAsync(SelectedGroupId, request);
                if (members == null)
                {
                    GroupMembers = [];
                    preferencesService.Set(GroupMembersCacheKey, string.Empty);
                    return;
                }

                FormatMembers(members);

                GroupMembers = members;
                preferencesService.Set(GroupMembersCacheKey, JsonSerializer.Serialize(members));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                IsMembersLoading = false;
            }
        }

        private void FormatMembers(IEnumerable<GroupMember> members)
        {
            var currentUserId = Guid.Empty;
            if (Guid.TryParse(preferencesService.Get(UserIdKey), out var parsedUserId))
            {
                currentUserId = parsedUserId;
            }

            foreach (var member in members)
            {
                member.DisplayUsername = member.Username;
                if (member.UserId == currentUserId && member.UserId == SelectedGroupOwnerId)
                {
                    member.DisplayUsername += " (Yo, Dueño)";
                }
                else if (member.UserId == SelectedGroupOwnerId)
                {
                    member.DisplayUsername += " (Dueño)";
                }
                else if (member.UserId == currentUserId)
                {
                    member.DisplayUsername += " (Yo)";
                }

                if (member.LastActivityAt.HasValue)
                {
                    member.LastActivityText = member.LastActivityAt.Value.ToString("dd/MM/yyyy - HH:mm");
                }
                else
                {
                    member.LastActivityText = "No Disponible";
                }

                if (member.Latitude.HasValue && member.Longitude.HasValue)
                {
                    member.LocationText = $"Lat: {member.Latitude.Value:0.00}, Lon: {member.Longitude.Value:0.00}";
                }
                else
                {
                    member.LocationText = "No Disponible";
                }

                if (member.SharingEnabled)
                {
                    member.Tags.Add("Compartiendo");
                }

                if (member.BlockedByMe)
                {
                    member.Tags.Add("Bloqueado");
                }

                if (member.BlockingMe)
                {
                    member.Tags.Add("Te bloqueó");
                }
            }
        }

        [RelayCommand]
        private async Task SelectGroupAsync(GroupDetails group)
        {
            IsGroupSelected = true;
            SelectedGroupId = group.GroupId;
            SelectedGroupName = group.Name;
            SelectedGroupOwnerId = group.OwnerUserId;

            preferencesService.Set(SelectedGroupIdKey, SelectedGroupId.ToString());
            preferencesService.Set(SelectedGroupNameKey, SelectedGroupName);
            preferencesService.Set(SelectedGroupOwnerIdKey, SelectedGroupOwnerId.ToString());

            await LoadGroupMembersAsync();
        }

        [RelayCommand]
        private void BackToGroups()
        {
            IsGroupSelected = false;
            SelectedGroupId = 0;
            SelectedGroupName = "Mis Grupos";
            SelectedGroupOwnerId = Guid.Empty;
            GroupMembers = [];

            preferencesService.Set(SelectedGroupIdKey, string.Empty);
            preferencesService.Set(SelectedGroupNameKey, string.Empty);
            preferencesService.Set(SelectedGroupOwnerIdKey, string.Empty);
        }
    }
}