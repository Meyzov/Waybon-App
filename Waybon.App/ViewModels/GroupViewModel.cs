using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Waybon.App.Data.Entities;
using Waybon.App.Data.Interfaces;
using Waybon.App.Models;
using Waybon.App.Services.Interfaces;

namespace Waybon.App.ViewModels
{
    public partial class GroupViewModel(ISessionService sessionService, IGroupService groupService, IPreferencesService preferencesService, IGroupRepository groupRepository, IGroupMemberRepository groupMemberRepository) : ObservableObject
    {
        private readonly ISessionService _sessionService = sessionService;
        private readonly IGroupService _groupService = groupService;
        private readonly IPreferencesService _preferencesService = preferencesService;
        private readonly IGroupRepository _groupRepository = groupRepository;
        private readonly IGroupMemberRepository _groupMemberRepository = groupMemberRepository;

        private const string SelectedGroupIdKey = "waybon_selectedGroupId";
        private const string SelectedGroupNameKey = "waybon_selectedGroupName";
        private const string SelectedGroupOwnerIdKey = "waybon_selectedGroupOwnerId";


        // ======================
        // Cancellation Sources
        // ======================

        private CancellationTokenSource? _groupLoadCts;
        private CancellationTokenSource? _memberLoadCts;


        // ======================
        // Groups
        // ======================

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AreGroupsVisible))]
        [NotifyPropertyChangedFor(nameof(IsNoGroupsMessageVisible))]
        [NotifyPropertyChangedFor(nameof(IsGroupsLoaderVisible))]
        public partial IEnumerable<GroupDetails> JoinedGroups { get; set; } = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AreGroupsVisible))]
        [NotifyPropertyChangedFor(nameof(IsNoGroupsMessageVisible))]
        [NotifyPropertyChangedFor(nameof(IsGroupsLoaderVisible))]
        public partial bool IsGroupsLoading { get; set; }

        private bool HasAnyGroups => JoinedGroups.Any();

        public bool AreGroupsVisible => HasAnyGroups && !IsGroupSelected;
        public bool IsNoGroupsMessageVisible => !HasAnyGroups && !IsGroupsLoading;
        public bool IsGroupsLoaderVisible => !HasAnyGroups && IsGroupsLoading;


        // ======================
        // Selected Group
        // ======================

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AreGroupsVisible))]
        public partial bool IsGroupSelected { get; set; }

        [ObservableProperty]
        public partial string SelectedGroupName { get; set; } = "Mis Grupos";

        public int SelectedGroupId { get; set; }
        public Guid SelectedGroupOwnerId { get; set; }


        // ======================
        // Group Members
        // ======================

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AreMembersVisible))]
        [NotifyPropertyChangedFor(nameof(IsNoMembersMessageVisible))]
        [NotifyPropertyChangedFor(nameof(IsMembersLoaderVisible))]
        public partial IEnumerable<GroupMember> GroupMembers { get; set; } = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AreMembersVisible))]
        [NotifyPropertyChangedFor(nameof(IsNoMembersMessageVisible))]
        [NotifyPropertyChangedFor(nameof(IsMembersLoaderVisible))]
        public partial bool IsMembersLoading { get; set; }

        private bool HasAnyMembers => GroupMembers.Any();

        public bool AreMembersVisible => HasAnyMembers && IsGroupSelected;
        public bool IsNoMembersMessageVisible => !HasAnyMembers && !IsMembersLoading;
        public bool IsMembersLoaderVisible => !HasAnyMembers && IsMembersLoading;


        // ======================
        // Initialization
        // ======================

        public async Task RestoreLastStateAsync()
        {
            try
            {
                var savedGroupIdText = _preferencesService.Get(SelectedGroupIdKey);
                var savedGroupName = _preferencesService.Get(SelectedGroupNameKey);
                var savedOwnerIdText = _preferencesService.Get(SelectedGroupOwnerIdKey);

                bool isValidId = int.TryParse(savedGroupIdText, out var savedGroupId);
                bool hasName = !string.IsNullOrEmpty(savedGroupName);
                bool isValidOwner = Guid.TryParse(savedOwnerIdText, out var savedOwnerId);

                if (!isValidId || !hasName || !isValidOwner)
                {
                    IsGroupSelected = false;

                    await LoadCachedGroupsAsync();
                    _ = RefreshGroupsAsync();

                    return;
                }

                IsGroupSelected = true;

                SelectedGroupId = savedGroupId;
                SelectedGroupName = savedGroupName;
                SelectedGroupOwnerId = savedOwnerId;

                await LoadCachedMembersAsync(SelectedGroupId, SelectedGroupOwnerId);
                _ = RefreshMembersAsync(SelectedGroupId, SelectedGroupOwnerId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error restoring last state: {ex.Message}");
            }
        }


        // ======================
        // Group Loading
        // ======================

        public async Task LoadCachedGroupsAsync()
        {
            _groupLoadCts?.Cancel();

            var cts = new CancellationTokenSource();
            _groupLoadCts = cts;

            IsGroupsLoading = true;

            try
            {
                var cachedGroups = await _groupRepository.GetGroupsAsync();
                if (cachedGroups.Count == 0)
                {
                    // ======================
                    // Abort if stale
                    // ======================

                    cts.Token.ThrowIfCancellationRequested();

                    JoinedGroups = [];
                    return;
                }

                var groups = MapToGroupDetails(cachedGroups);
                FormatGroupsForDisplay(groups);

                // ======================
                // Abort if stale
                // ======================

                cts.Token.ThrowIfCancellationRequested();

                JoinedGroups = groups.OrderBy(g => g.Name);
            }
            catch (OperationCanceledException)
            {
                // Ignore
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading cached groups: {ex.Message}");
            }
            finally
            {
                if (_groupLoadCts == cts)
                {
                    IsGroupsLoading = false;
                    _groupLoadCts = null;
                }

                cts.Dispose();
            }
        }

        public async Task RefreshGroupsAsync()
        {
            _groupLoadCts?.Cancel();

            var cts = new CancellationTokenSource();
            _groupLoadCts = cts;

            IsGroupsLoading = true;

            try
            {
                if (_sessionService.SessionId == Guid.Empty)
                {
                    return;
                }

                var request = new SessionIdRequest { SessionId = _sessionService.SessionId };
                var groups = await _groupService.GetJoinedGroupsAsync(request, _groupLoadCts.Token);

                if (groups == null || !groups.Any())
                {
                    // ======================
                    // Abort if stale
                    // ======================

                    cts.Token.ThrowIfCancellationRequested();

                    JoinedGroups = [];

                    await _groupMemberRepository.ClearAllMembersAsync();
                    await _groupRepository.ClearAllGroupsAsync();

                    return;
                }

                FormatGroupsForDisplay(groups);

                // ======================
                // Abort if stale
                // ======================

                cts.Token.ThrowIfCancellationRequested();

                JoinedGroups = groups.OrderBy(g => g.Name);
                await _groupRepository.SaveGroupsAsync(MapToLocalGroups(groups));
            }
            catch (OperationCanceledException)
            {
                // Ignore
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading groups: {ex.Message}");
            }
            finally
            {
                if (_groupLoadCts == cts)
                {
                    IsGroupsLoading = false;
                    _groupLoadCts = null;
                }

                cts.Dispose();
            }
        }


        // ======================
        // Member Loading
        // ======================

        public async Task LoadCachedMembersAsync(int selectedGroupId, Guid selectedGroupOwnerId)
        {
            _memberLoadCts?.Cancel();

            var cts = new CancellationTokenSource();
            _memberLoadCts = cts;

            IsMembersLoading = true;

            try
            {
                var cachedMembers = await _groupMemberRepository.GetMembersAsync(selectedGroupId);
                if (cachedMembers.Count == 0)
                {
                    // ======================
                    // Abort if stale
                    // ======================

                    cts.Token.ThrowIfCancellationRequested();

                    GroupMembers = [];
                    return;
                }
                
                var members = MapToGroupMembers(cachedMembers);
                FormatMembersForDisplay(selectedGroupOwnerId, members);

                // ======================
                // Abort if stale
                // ======================

                cts.Token.ThrowIfCancellationRequested();

                GroupMembers = members.OrderBy(m => m.Username);
            }
            catch (OperationCanceledException)
            {
                // Ignore
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading cached members: {ex.Message}");
            }
            finally
            {
                if (_memberLoadCts == cts)
                {
                    IsMembersLoading = false;
                    _memberLoadCts = null;
                }

                cts.Dispose();
            }
        }

        public async Task RefreshMembersAsync(int selectedGroupId, Guid selectedGroupOwnerId)
        {
            _memberLoadCts?.Cancel();

            var cts = new CancellationTokenSource();
            _memberLoadCts = cts;

            IsMembersLoading = true;

            try
            {
                if (_sessionService.SessionId == Guid.Empty)
                {
                    return;
                }

                var request = new SessionIdRequest { SessionId = _sessionService.SessionId };
                var members = await _groupService.GetGroupMembersAsync(selectedGroupId, request, _memberLoadCts.Token);

                if (members == null || !members.Any())
                {
                    // ======================
                    // Abort if stale
                    // ======================

                    cts.Token.ThrowIfCancellationRequested();

                    GroupMembers = [];
                    await _groupMemberRepository.ClearMembersAsync(selectedGroupId);

                    return;
                }

                FormatMembersForDisplay(selectedGroupOwnerId, members);

                // ======================
                // Abort if stale
                // ======================

                cts.Token.ThrowIfCancellationRequested();

                GroupMembers = members.OrderBy(m => m.Username);
                await _groupMemberRepository.SaveMembersAsync(selectedGroupId, MapToLocalMembers(selectedGroupId, members));
            }
            catch (OperationCanceledException)
            {
                // Ignore
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading group members: {ex.Message}");
            }
            finally
            {
                if (_memberLoadCts == cts)
                {
                    IsMembersLoading = false;
                    _memberLoadCts = null;
                }

                cts.Dispose();
            }
        }


        // ======================
        // Display Formatting
        // ======================

        private void FormatGroupsForDisplay(IEnumerable<GroupDetails> groups)
        {
            var currentUserId = _sessionService.UserId;

            foreach (var group in groups)
            {
                group.DisplayUsername = (group.OwnerUserId == currentUserId) ? $"{group.Username} (Yo)" : group.Username;
            }
        }

        private void FormatMembersForDisplay(Guid selectedGroupOwnerId, IEnumerable<GroupMember> members)
        {
            var currentUserId = _sessionService.UserId;

            foreach (var member in members)
            {
                member.DisplayUsername = BuildMemberDisplayName(currentUserId, selectedGroupOwnerId, member);
                BuildMemberTags(member);
            }
        }

        private static string BuildMemberDisplayName(Guid currentUserId, Guid selectedGroupOwnerId, GroupMember member)
        {
            var isCurrentUser = member.UserId == currentUserId;
            var isOwner = member.UserId == selectedGroupOwnerId;

            if (isCurrentUser && isOwner)
            {
                return $"{member.Username} (Yo, Dueño)";
            }

            if (isOwner)
            {
                return $"{member.Username} (Dueño)";
            }

            if (isCurrentUser)
            {
                return $"{member.Username} (Yo)";
            }

            return member.Username;
        }

        private static void BuildMemberTags(GroupMember member)
        {
            member.Tags.Clear();

            if (member.SharingEnabled)
            {
                member.Tags.Add("Compartiendo");
            }
            else
            {
                member.Tags.Add("No Compartiendo");
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


        // ======================
        // Commands
        // ======================

        [RelayCommand]
        private async Task SelectGroupAsync(GroupDetails group)
        {
            GroupMembers = [];

            SelectedGroupId = group.GroupId;
            SelectedGroupName = group.Name;
            SelectedGroupOwnerId = group.OwnerUserId;

            _preferencesService.Set(SelectedGroupIdKey, group.GroupId.ToString());
            _preferencesService.Set(SelectedGroupNameKey, group.Name);
            _preferencesService.Set(SelectedGroupOwnerIdKey, group.OwnerUserId.ToString());

            IsGroupSelected = true;

            await LoadCachedMembersAsync(group.GroupId, group.OwnerUserId);
            _ = RefreshMembersAsync(group.GroupId, group.OwnerUserId);
        }

        [RelayCommand]
        private async Task BackToGroupsAsync()
        {
            ClearSelectedGroupState();
            GroupMembers = [];

            await LoadCachedGroupsAsync();
            _ = RefreshGroupsAsync();
        }

        private void ClearSelectedGroupState()
        {
            IsGroupSelected = false;
            SelectedGroupId = 0;
            SelectedGroupName = "Mis Grupos";
            SelectedGroupOwnerId = Guid.Empty;

            _preferencesService.Set(SelectedGroupIdKey, string.Empty);
            _preferencesService.Set(SelectedGroupNameKey, string.Empty);
            _preferencesService.Set(SelectedGroupOwnerIdKey, string.Empty);
        }


        // ======================
        // Mappers
        // ======================

        private static List<GroupDetails> MapToGroupDetails(List<LocalGroup> groups)
        {
            return
            [
                .. groups.Select(g => new GroupDetails
                {
                    GroupId = g.GroupId,
                    OwnerUserId = g.OwnerUserId,
                    Username = g.Username,
                    Name = g.Name,
                    JoinCode = g.JoinCode,
                    JoinCodeExpiresAt = g.JoinCodeExpiresAt,
                    CreatedAt = g.CreatedAt
                })
            ];
        }

        private static List<LocalGroup> MapToLocalGroups(IEnumerable<GroupDetails> groups)
        {
            return
            [
                .. groups.Select(g => new LocalGroup
                {
                    GroupId = g.GroupId,
                    OwnerUserId = g.OwnerUserId,
                    Username = g.Username,
                    Name = g.Name,
                    JoinCode = g.JoinCode,
                    JoinCodeExpiresAt = g.JoinCodeExpiresAt,
                    CreatedAt = g.CreatedAt
                })
            ];
        }

        private static List<GroupMember> MapToGroupMembers(List<LocalMember> members)
        {
            return
            [
                .. members.Select(m => new GroupMember
                {
                    UserId = m.UserId,
                    Username = m.Username,
                    SharingEnabled = m.SharingEnabled,
                    BlockedByMe = m.BlockedByMe,
                    BlockingMe = m.BlockingMe,
                    LastActivityAt = m.LastActivityAt
                })
            ];
        }

        private static List<LocalMember> MapToLocalMembers(int groupId, IEnumerable<GroupMember> members)
        {
            return
            [
                .. members.Select(m => new LocalMember
                {
                    GroupId = groupId,
                    UserId = m.UserId,
                    Username = m.Username,
                    SharingEnabled = m.SharingEnabled,
                    BlockedByMe = m.BlockedByMe,
                    BlockingMe = m.BlockingMe,
                    LastActivityAt = m.LastActivityAt
                })
            ];
        }
    }
}