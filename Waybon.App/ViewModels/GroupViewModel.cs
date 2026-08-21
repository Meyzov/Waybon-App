using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Waybon.App.Data.Entities;
using Waybon.App.Data.Interfaces;
using Waybon.App.Models;
using Waybon.App.Services.Interfaces;

namespace Waybon.App.ViewModels
{
    public partial class GroupViewModel(ISessionService sessionService, IGroupService groupService, IPreferencesService preferencesService, IGroupRepository groupRepository, IGroupMemberRepository groupMemberRepository, IDialogService dialogService) : ObservableObject
    {
        private readonly ISessionService _sessionService = sessionService;
        private readonly IGroupService _groupService = groupService;
        private readonly IPreferencesService _preferencesService = preferencesService;
        private readonly IGroupRepository _groupRepository = groupRepository;
        private readonly IGroupMemberRepository _groupMemberRepository = groupMemberRepository;
        private readonly IDialogService _dialogService = dialogService;

        private const string SelectedGroupIdKey = "waybon_selectedGroupId";
        private const string SelectedGroupNameKey = "waybon_selectedGroupName";
        private const string SelectedGroupOwnerIdKey = "waybon_selectedGroupOwnerId";
        private const string SelectedGroupJoinCodeKey = "waybon_selectedGroupJoinCode";


        // ======================
        // Cancellation Sources
        // ======================

        private CancellationTokenSource? _groupLoadCts;
        private CancellationTokenSource? _memberLoadCts;


        // ======================
        // Groups
        // ======================

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNoGroupsMessageVisible))]
        [NotifyPropertyChangedFor(nameof(IsGroupsLoaderVisible))]
        public partial ObservableCollection<GroupDetails> JoinedGroups { get; set; } = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNoGroupsMessageVisible))]
        [NotifyPropertyChangedFor(nameof(IsGroupsLoaderVisible))]
        public partial bool IsGroupsLoading { get; set; }

        private bool HasAnyGroups => JoinedGroups.Count > 0;

        public bool IsNoGroupsMessageVisible => AreGroupsVisible && !HasAnyGroups && !IsGroupsLoading;
        public bool IsGroupsLoaderVisible => AreGroupsVisible && !HasAnyGroups && IsGroupsLoading;


        // ======================
        // Paneles
        // ======================

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNoGroupsMessageVisible))]
        [NotifyPropertyChangedFor(nameof(IsGroupsLoaderVisible))]
        public partial bool AreGroupsVisible { get; set; } = true;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNoMembersMessageVisible))]
        [NotifyPropertyChangedFor(nameof(IsMembersLoaderVisible))]
        public partial bool AreMembersVisible { get; set; }

        [ObservableProperty]
        public partial bool IsCreateGroupPanelVisible { get; set; }

        [ObservableProperty]
        public partial bool IsJoinGroupPanelVisible { get; set; }

        [ObservableProperty]
        public partial bool IsGroupSettingsVisible { get; set; }


        // ======================
        // Selected Group
        // ======================

        [ObservableProperty]
        public partial bool IsGroupSelected { get; set; }

        [ObservableProperty]
        public partial string SelectedGroupName { get; set; } = "Mis Grupos";

        public int SelectedGroupId { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCurrentUserOwner))]
        public partial Guid SelectedGroupOwnerId { get; set; }

        [ObservableProperty]
        public partial string SelectedGroupJoinCode { get; set; } = string.Empty;

        public bool IsCurrentUserOwner => SelectedGroupOwnerId != Guid.Empty && SelectedGroupOwnerId == _sessionService.UserId;


        // ======================
        // Group Panels Inputs
        // ======================

        [ObservableProperty]
        public partial string NewGroupName { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string JoinCodeInput { get; set; } = string.Empty;


        // ======================
        // Group Members
        // ======================

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNoMembersMessageVisible))]
        [NotifyPropertyChangedFor(nameof(IsMembersLoaderVisible))]
        public partial ObservableCollection<GroupMember> GroupMembers { get; set; } = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNoMembersMessageVisible))]
        [NotifyPropertyChangedFor(nameof(IsMembersLoaderVisible))]
        public partial bool IsMembersLoading { get; set; }

        private bool HasAnyMembers => GroupMembers.Count > 0;

        public bool IsNoMembersMessageVisible => AreMembersVisible && !HasAnyMembers && !IsMembersLoading;
        public bool IsMembersLoaderVisible => AreMembersVisible && !HasAnyMembers && IsMembersLoading;


        // ======================
        // Panel Switching
        // ======================

        private void SwitchToGroupsPanel()
        {
            AreGroupsVisible = true;
            AreMembersVisible = false;
            IsCreateGroupPanelVisible = false;
            IsJoinGroupPanelVisible = false;
            IsGroupSettingsVisible = false;
        }

        private void SwitchToMembersPanel()
        {
            AreGroupsVisible = false;
            AreMembersVisible = true;
            IsCreateGroupPanelVisible = false;
            IsJoinGroupPanelVisible = false;
            IsGroupSettingsVisible = false;
        }

        private void SwitchToCreateGroupPanel()
        {
            AreGroupsVisible = false;
            AreMembersVisible = false;
            IsCreateGroupPanelVisible = true;
            IsJoinGroupPanelVisible = false;
            IsGroupSettingsVisible = false;
        }

        private void SwitchToJoinGroupPanel()
        {
            AreGroupsVisible = false;
            AreMembersVisible = false;
            IsCreateGroupPanelVisible = false;
            IsJoinGroupPanelVisible = true;
            IsGroupSettingsVisible = false;
        }

        private void SwitchToGroupSettingsPanel()
        {
            AreGroupsVisible = false;
            AreMembersVisible = false;
            IsCreateGroupPanelVisible = false;
            IsJoinGroupPanelVisible = false;
            IsGroupSettingsVisible = true;
        }


        // ======================
        // Initialization
        // ======================

        public async Task RestoreLastStateAsync()
        {
            try
            {
                if (!TryGetSavedGroupState())
                {
                    SwitchToGroupsPanel();

                    await LoadCachedGroupsAsync();
                    _ = RefreshGroupsAsync();

                    return;
                }

                SwitchToMembersPanel();

                await LoadCachedMembersAsync(SelectedGroupId, SelectedGroupOwnerId);
                _ = RefreshMembersAsync(SelectedGroupId, SelectedGroupOwnerId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error restoring last state: {ex.Message}");
            }
        }

        private bool TryGetSavedGroupState()
        {
            var groupIdText = _preferencesService.Get(SelectedGroupIdKey);
            var savedGroupName = _preferencesService.Get(SelectedGroupNameKey);
            var ownerIdText = _preferencesService.Get(SelectedGroupOwnerIdKey);
            var savedJoinCode = _preferencesService.Get(SelectedGroupJoinCodeKey);

            bool isValidId = int.TryParse(groupIdText, out int savedGroupId);
            bool isValidOwner = Guid.TryParse(ownerIdText, out Guid savedOwnerId);

            if (isValidId && !string.IsNullOrEmpty(savedGroupName) && isValidOwner)
            {
                IsGroupSelected = true;
                SelectedGroupId = savedGroupId;
                SelectedGroupName = savedGroupName;
                SelectedGroupOwnerId = savedOwnerId;
                SelectedGroupJoinCode = savedJoinCode;

                return true;
            }

            IsGroupSelected = false;
            return false;
        }


        // ======================
        // Group Loading
        // ======================

        public async Task LoadCachedGroupsAsync()
        {
            var cts = StartGroupLoad();
            var currentUserId = _sessionService.UserId;

            try
            {
                var cachedGroups = await _groupRepository.GetGroupsAsync();
                if (!cachedGroups.Any())
                {
                    // ======================
                    // Abort if stale
                    // ======================

                    cts.Token.ThrowIfCancellationRequested();

                    JoinedGroups.Clear();
                    OnPropertyChanged(nameof(IsNoGroupsMessageVisible));
                    OnPropertyChanged(nameof(IsGroupsLoaderVisible));

                    return;
                }

                var groups = MapToGroupDetails(cachedGroups);
                FormatGroupsForDisplay(currentUserId, groups);

                // ======================
                // Abort if stale
                // ======================

                cts.Token.ThrowIfCancellationRequested();

                var incomingIds = groups.Select(g => g.GroupId).ToHashSet();
                var groupsToRemove = JoinedGroups.Where(g => !incomingIds.Contains(g.GroupId)).ToList();

                foreach (var group in groupsToRemove)
                {
                    JoinedGroups.Remove(group);
                    OnPropertyChanged(nameof(IsNoGroupsMessageVisible));
                    OnPropertyChanged(nameof(IsGroupsLoaderVisible));
                }

                foreach (var group in groups.OrderBy(g => g.Name))
                {
                    var existingGroup = JoinedGroups.FirstOrDefault(g => g.GroupId == group.GroupId);
                    if (existingGroup != null)
                    {
                        existingGroup.Name = group.Name;
                        existingGroup.Username = group.Username;
                        existingGroup.DisplayUsername = group.DisplayUsername;
                        existingGroup.JoinCode = group.JoinCode;
                        existingGroup.JoinCodeExpiresAt = group.JoinCodeExpiresAt;
                        existingGroup.CreatedAt = group.CreatedAt;

                        continue;
                    }

                    int index = 0;
                    while (index < JoinedGroups.Count && string.Compare(JoinedGroups[index].Name, group.Name, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        index++;
                    }

                    JoinedGroups.Insert(index, group);
                    OnPropertyChanged(nameof(IsNoGroupsMessageVisible));
                    OnPropertyChanged(nameof(IsGroupsLoaderVisible));
                }
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
                FinishGroupLoad(cts);
            }
        }

        public async Task RefreshGroupsAsync()
        {
            var cts = StartGroupLoad();

            var currentSessionId = _sessionService.SessionId;
            var currentUserId = _sessionService.UserId;

            try
            {
                var groups = await FetchGroupsAsync(currentSessionId, cts.Token);
                if (groups.Count == 0)
                {
                    // ======================
                    // Abort if stale
                    // ======================

                    cts.Token.ThrowIfCancellationRequested();

                    JoinedGroups.Clear();
                    OnPropertyChanged(nameof(IsNoGroupsMessageVisible));
                    OnPropertyChanged(nameof(IsGroupsLoaderVisible));

                    await _groupMemberRepository.ClearAllMembersAsync();
                    await _groupRepository.ClearAllGroupsAsync();

                    return;
                }

                var localGroupsToSave = MapToLocalGroups(groups);
                FormatGroupsForDisplay(currentUserId, groups);

                // ======================
                // Abort if stale
                // ======================

                cts.Token.ThrowIfCancellationRequested();

                var incomingIds = groups.Select(g => g.GroupId).ToHashSet();
                var groupsToRemove = JoinedGroups.Where(g => !incomingIds.Contains(g.GroupId)).ToList();

                foreach (var group in groupsToRemove)
                {
                    JoinedGroups.Remove(group);
                    OnPropertyChanged(nameof(IsNoGroupsMessageVisible));
                    OnPropertyChanged(nameof(IsGroupsLoaderVisible));
                }

                foreach (var group in groups.OrderBy(g => g.Name))
                {
                    var existingGroup = JoinedGroups.FirstOrDefault(g => g.GroupId == group.GroupId);
                    if (existingGroup != null)
                    {
                        existingGroup.Name = group.Name;
                        existingGroup.Username = group.Username;
                        existingGroup.DisplayUsername = group.DisplayUsername;
                        existingGroup.JoinCode = group.JoinCode;
                        existingGroup.JoinCodeExpiresAt = group.JoinCodeExpiresAt;
                        existingGroup.CreatedAt = group.CreatedAt;

                        continue;
                    }

                    int index = 0;
                    while (index < JoinedGroups.Count && string.Compare(JoinedGroups[index].Name, group.Name, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        index++;
                    }

                    JoinedGroups.Insert(index, group);
                    OnPropertyChanged(nameof(IsNoGroupsMessageVisible));
                    OnPropertyChanged(nameof(IsGroupsLoaderVisible));
                }

                await _groupRepository.SaveGroupsAsync(localGroupsToSave);
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
                FinishGroupLoad(cts);
            }
        }

        private CancellationTokenSource StartGroupLoad()
        {
            _groupLoadCts?.Cancel();
            var cts = new CancellationTokenSource();
            _groupLoadCts = cts;

            IsGroupsLoading = true;
            return cts;
        }

        private void FinishGroupLoad(CancellationTokenSource cts)
        {
            if (_groupLoadCts == cts)
            {
                IsGroupsLoading = false;
                _groupLoadCts = null;
            }

            cts.Dispose();
        }

        private async Task<List<GroupDetails>> FetchGroupsAsync(Guid sessionId, CancellationToken cancellationToken)
        {
            if (sessionId == Guid.Empty)
            {
                return [];
            }

            var request = new SessionIdRequest
            {
                SessionId = sessionId
            };

            return [.. await _groupService.GetJoinedGroupsAsync(request, cancellationToken)];
        }


        // ======================
        // Member Loading
        // ======================

        public async Task LoadCachedMembersAsync(int selectedGroupId, Guid selectedGroupOwnerId)
        {
            var cts = StartMemberLoad();
            var currentUserId = _sessionService.UserId;

            try
            {
                var cachedMembers = await _groupMemberRepository.GetMembersAsync(selectedGroupId);
                if (!cachedMembers.Any())
                {
                    // ======================
                    // Abort if stale
                    // ======================

                    cts.Token.ThrowIfCancellationRequested();

                    GroupMembers.Clear();
                    OnPropertyChanged(nameof(IsNoMembersMessageVisible));
                    OnPropertyChanged(nameof(IsMembersLoaderVisible));

                    return;
                }

                var members = MapToGroupMembers(cachedMembers);
                FormatMembersForDisplay(currentUserId, selectedGroupOwnerId, members);

                // ======================
                // Abort if stale
                // ======================

                cts.Token.ThrowIfCancellationRequested();

                var incomingIds = members.Select(m => m.UserId).ToHashSet();
                var membersToRemove = GroupMembers.Where(m => !incomingIds.Contains(m.UserId)).ToList();

                foreach (var member in membersToRemove)
                {
                    GroupMembers.Remove(member);
                    OnPropertyChanged(nameof(IsNoMembersMessageVisible));
                    OnPropertyChanged(nameof(IsMembersLoaderVisible));
                }

                foreach (var member in members.OrderBy(m => m.Username))
                {
                    var existingMember = GroupMembers.FirstOrDefault(m => m.UserId == member.UserId);
                    if (existingMember != null)
                    {
                        existingMember.Username = member.Username;
                        existingMember.DisplayUsername = member.DisplayUsername;
                        existingMember.SharingEnabled = member.SharingEnabled;
                        existingMember.BlockedByMe = member.BlockedByMe;
                        existingMember.BlockingMe = member.BlockingMe;
                        existingMember.LastActivityAt = member.LastActivityAt;

                        BuildMemberTags(existingMember);

                        continue;
                    }

                    int index = 0;
                    while (index < GroupMembers.Count && string.Compare(GroupMembers[index].Username, member.Username, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        index++;
                    }

                    GroupMembers.Insert(index, member);
                    OnPropertyChanged(nameof(IsNoMembersMessageVisible));
                    OnPropertyChanged(nameof(IsMembersLoaderVisible));
                }
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
                FinishMemberLoad(cts);
            }
        }

        public async Task RefreshMembersAsync(int selectedGroupId, Guid selectedGroupOwnerId)
        {
            var cts = StartMemberLoad();

            var currentSessionId = _sessionService.SessionId;
            var currentUserId = _sessionService.UserId;

            try
            {
                var members = await FetchMembersAsync(currentSessionId, selectedGroupId, cts.Token);
                if (members.Count == 0)
                {
                    // ======================
                    // Abort if stale
                    // ======================

                    cts.Token.ThrowIfCancellationRequested();

                    GroupMembers.Clear();
                    OnPropertyChanged(nameof(IsNoMembersMessageVisible));
                    OnPropertyChanged(nameof(IsMembersLoaderVisible));

                    await _groupMemberRepository.ClearMembersAsync(selectedGroupId);

                    return;
                }

                var localMembersToSave = MapToLocalMembers(selectedGroupId, members);
                FormatMembersForDisplay(currentUserId, selectedGroupOwnerId, members);

                // ======================
                // Abort if stale
                // ======================

                cts.Token.ThrowIfCancellationRequested();

                var incomingIds = members.Select(m => m.UserId).ToHashSet();
                var membersToRemove = GroupMembers.Where(m => !incomingIds.Contains(m.UserId)).ToList();

                foreach (var member in membersToRemove)
                {
                    GroupMembers.Remove(member);
                    OnPropertyChanged(nameof(IsNoMembersMessageVisible));
                    OnPropertyChanged(nameof(IsMembersLoaderVisible));
                }

                foreach (var member in members.OrderBy(m => m.Username))
                {
                    var existingMember = GroupMembers.FirstOrDefault(m => m.UserId == member.UserId);
                    if (existingMember != null)
                    {
                        existingMember.Username = member.Username;
                        existingMember.DisplayUsername = member.DisplayUsername;
                        existingMember.SharingEnabled = member.SharingEnabled;
                        existingMember.BlockedByMe = member.BlockedByMe;
                        existingMember.BlockingMe = member.BlockingMe;
                        existingMember.LastActivityAt = member.LastActivityAt;

                        BuildMemberTags(existingMember);

                        continue;
                    }

                    int index = 0;
                    while (index < GroupMembers.Count && string.Compare(GroupMembers[index].Username, member.Username, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        index++;
                    }

                    GroupMembers.Insert(index, member);
                    OnPropertyChanged(nameof(IsNoMembersMessageVisible));
                    OnPropertyChanged(nameof(IsMembersLoaderVisible));
                }

                await _groupMemberRepository.SaveMembersAsync(selectedGroupId, localMembersToSave);
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
                FinishMemberLoad(cts);
            }
        }

        private CancellationTokenSource StartMemberLoad()
        {
            _memberLoadCts?.Cancel();
            var cts = new CancellationTokenSource();
            _memberLoadCts = cts;

            IsMembersLoading = true;
            return cts;
        }

        private void FinishMemberLoad(CancellationTokenSource cts)
        {
            if (_memberLoadCts == cts)
            {
                IsMembersLoading = false;
                _memberLoadCts = null;
            }

            cts.Dispose();
        }

        private async Task<List<GroupMember>> FetchMembersAsync(Guid sessionId, int groupId, CancellationToken cancellationToken)
        {
            if (sessionId == Guid.Empty)
            {
                return [];
            }

            var request = new SessionIdRequest
            {
                SessionId = sessionId
            };

            return [.. await _groupService.GetGroupMembersAsync(groupId, request, cancellationToken)];
        }


        // ======================
        // Display Formatting
        // ======================

        private static void FormatGroupsForDisplay(Guid currentUserId, IEnumerable<GroupDetails> groups)
        {
            foreach (var group in groups)
            {
                group.DisplayUsername = (group.OwnerUserId == currentUserId)
                    ? $"{group.Username} (Yo)"
                    : group.Username;

                group.CreatedAt = ToLocalTime(group.CreatedAt);
            }
        }

        private static void FormatMembersForDisplay(Guid currentUserId, Guid selectedGroupOwnerId, IEnumerable<GroupMember> members)
        {
            foreach (var member in members)
            {
                member.DisplayUsername = BuildMemberDisplayName(currentUserId, selectedGroupOwnerId, member);
                member.LastActivityAt = ToLocalTime(member.LastActivityAt);
                BuildMemberTags(member);
            }
        }

        private static string BuildMemberDisplayName(Guid currentUserId, Guid selectedGroupOwnerId, GroupMember member)
        {
            var isCurrentUser = member.UserId == currentUserId;
            var isOwner = member.UserId == selectedGroupOwnerId;

            if (isCurrentUser && isOwner) return $"{member.Username} (Yo, Dueño)";
            if (isOwner) return $"{member.Username} (Dueño)";
            if (isCurrentUser) return $"{member.Username} (Yo)";

            return member.Username;
        }

        private static void BuildMemberTags(GroupMember member)
        {
            member.Tags.Clear();

            member.Tags.Add(member.SharingEnabled ? "Compartiendo" : "No Compartiendo");

            if (member.BlockedByMe) member.Tags.Add("Bloqueado");
            if (member.BlockingMe) member.Tags.Add("Te bloqueó");
        }


        // ======================
        // Time Helpers
        // ======================

        private static DateTime ToLocalTime(DateTime date) => DateTime.SpecifyKind(date, DateTimeKind.Utc).ToLocalTime();
        private static DateTime? ToLocalTime(DateTime? date) => date.HasValue ? ToLocalTime(date.Value) : null;


        // ======================
        // Commands
        // ======================

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task SelectGroupAsync(GroupDetails group)
        {
            GroupMembers.Clear();
            OnPropertyChanged(nameof(IsNoMembersMessageVisible));
            OnPropertyChanged(nameof(IsMembersLoaderVisible));

            SelectedGroupId = group.GroupId;
            SelectedGroupName = group.Name;
            SelectedGroupOwnerId = group.OwnerUserId;
            SelectedGroupJoinCode = group.JoinCode ?? string.Empty;

            _preferencesService.Set(SelectedGroupIdKey, group.GroupId.ToString());
            _preferencesService.Set(SelectedGroupNameKey, group.Name);
            _preferencesService.Set(SelectedGroupOwnerIdKey, group.OwnerUserId.ToString());
            _preferencesService.Set(SelectedGroupJoinCodeKey, group.JoinCode ?? string.Empty);

            IsGroupSelected = true;
            SwitchToMembersPanel();

            await LoadCachedMembersAsync(group.GroupId, group.OwnerUserId);
            _ = RefreshMembersAsync(group.GroupId, group.OwnerUserId);
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task BackToGroupsAsync()
        {
            await LoadCachedGroupsAsync();
            _ = RefreshGroupsAsync();

            ClearSelectedGroupState();
            SwitchToGroupsPanel();

            GroupMembers.Clear();
            OnPropertyChanged(nameof(IsNoMembersMessageVisible));
            OnPropertyChanged(nameof(IsMembersLoaderVisible));
        }

        private void ClearSelectedGroupState()
        {
            IsGroupSelected = false;
            SelectedGroupId = 0;
            SelectedGroupName = "Mis Grupos";
            SelectedGroupOwnerId = Guid.Empty;
            SelectedGroupJoinCode = string.Empty;

            _preferencesService.Set(SelectedGroupIdKey, string.Empty);
            _preferencesService.Set(SelectedGroupNameKey, string.Empty);
            _preferencesService.Set(SelectedGroupOwnerIdKey, string.Empty);
            _preferencesService.Set(SelectedGroupJoinCodeKey, string.Empty);
        }

        [RelayCommand]
        private void ShowCreateGroup()
        {
            SwitchToCreateGroupPanel();
            NewGroupName = string.Empty;
        }

        [RelayCommand]
        private void ShowJoinGroup()
        {
            SwitchToJoinGroupPanel();
            JoinCodeInput = string.Empty;
        }

        [RelayCommand]
        private void CancelCreateGroup()
        {
            SwitchToGroupsPanel();
            NewGroupName = string.Empty;
        }

        [RelayCommand]
        private void CancelJoinGroup()
        {
            SwitchToGroupsPanel();
            JoinCodeInput = string.Empty;
        }

        [RelayCommand]
        private void ShowGroupSettings()
        {
            SwitchToGroupSettingsPanel();
        }

        [RelayCommand]
        private void HideGroupSettings()
        {
            SwitchToMembersPanel();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task CreateGroupAsync()
        {
            string groupName = NewGroupName;
            Guid sessionId = _sessionService.SessionId;

            if (string.IsNullOrWhiteSpace(groupName))
            {
                return;
            }

            try
            {
                if (!await SendCreateGroupRequestAsync(sessionId, groupName))
                {
                    await _dialogService.ShowAlertAsync("No se pudo crear", "Ocurrió un error al intentar crear el grupo. Inténtalo de nuevo.", "Ok");
                    return;
                }

                await HandleCreateGroupSuccessAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating group: {ex.Message}");
            }
        }

        private async Task<bool> SendCreateGroupRequestAsync(Guid sessionId, string name)
        {
            var request = new CreateGroupRequest
            {
                SessionId = sessionId,
                Name = name.Trim()
            };

            return await _groupService.CreateGroupAsync(request);
        }

        private async Task HandleCreateGroupSuccessAsync()
        {
            await LoadCachedGroupsAsync();
            _ = RefreshGroupsAsync();

            await _dialogService.ShowAlertAsync("¡Listo!", "El grupo ha sido creado satisfactoriamente.", "Ok");

            SwitchToGroupsPanel();
            NewGroupName = string.Empty;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task JoinGroupAsync()
        {
            string currentJoinCode = JoinCodeInput;
            Guid currentSessionId = _sessionService.SessionId;

            if (string.IsNullOrWhiteSpace(currentJoinCode))
            {
                return;
            }

            try
            {
                if (!await SendJoinGroupRequestAsync(currentSessionId, currentJoinCode))
                {
                    await _dialogService.ShowAlertAsync("No se pudo unir", "Ocurrió un error al intentar unirte al grupo. Verifica el código e inténtalo de nuevo.", "Ok");
                    return;
                }

                await HandleJoinGroupSuccessAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error joining group: {ex.Message}");
            }
        }

        private async Task<bool> SendJoinGroupRequestAsync(Guid sessionId, string joinCode)
        {
            var request = new JoinGroupRequest
            {
                SessionId = sessionId,
                JoinCode = joinCode.Trim()
            };

            return await _groupService.JoinGroupAsync(request);
        }

        private async Task HandleJoinGroupSuccessAsync()
        {
            await LoadCachedGroupsAsync();
            _ = RefreshGroupsAsync();

            await _dialogService.ShowAlertAsync("¡Listo!", "Te has unido al grupo satisfactoriamente.", "Ok");

            SwitchToGroupsPanel();
            JoinCodeInput = string.Empty;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task RegenerateJoinCodeAsync()
        {
            var groupId = SelectedGroupId;

            try
            {
                var response = await RequestNewJoinCodeAsync();
                if (response == null)
                {
                    await _dialogService.ShowAlertAsync("Error", "No se pudo regenerar el código. Inténtalo de nuevo.", "Ok");
                    return;
                }

                await HandleRegenerateJoinCodeSuccessAsync(response, groupId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error regenerating join code: {ex.Message}");
                await _dialogService.ShowAlertAsync("Error", "No se pudo regenerar el código. Inténtalo de nuevo.", "Ok");
            }
        }

        private async Task<RegenerateJoinCodeResponse?> RequestNewJoinCodeAsync()
        {
            var request = new SessionIdRequest
            {
                SessionId = _sessionService.SessionId
            };

            return await _groupService.RegenerateJoinCodeAsync(SelectedGroupId, request);
        }

        private async Task HandleRegenerateJoinCodeSuccessAsync(RegenerateJoinCodeResponse response, int groupId)
        {
            ApplyNewJoinCode(response.JoinCode, groupId);

            await SaveJoinCodeToCacheAsync(response, groupId);
            await ShowNewJoinCodeDialogAsync(response);
        }

        private void ApplyNewJoinCode(string? joinCode, int groupId)
        {
            if (groupId != SelectedGroupId)
            {
                return;
            }

            SelectedGroupJoinCode = joinCode ?? string.Empty;
            _preferencesService.Set(SelectedGroupJoinCodeKey, joinCode ?? string.Empty);
        }

        private async Task SaveJoinCodeToCacheAsync(RegenerateJoinCodeResponse response, int groupId)
        {
            var localGroup = await _groupRepository.GetGroupByIdAsync(groupId);
            if (localGroup == null)
            {
                return;
            }

            localGroup.JoinCode = response.JoinCode;
            localGroup.JoinCodeExpiresAt = response.JoinCodeExpiresAt;

            await _groupRepository.SaveGroupAsync(localGroup);
        }

        private async Task ShowNewJoinCodeDialogAsync(RegenerateJoinCodeResponse response)
        {
            var localExpiresAt = ToLocalTime(response.JoinCodeExpiresAt);
            var expirationText = localExpiresAt.HasValue
                ? $"Expira el: {localExpiresAt.Value:dd/MM/yyyy HH:mm}"
                : "No disponible";

            await _dialogService.ShowAlertAsync("¡Listo!", $"El código de invitación ha sido regenerado.\n\nCódigo: {response.JoinCode}\n{expirationText}", "Ok");
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task DeleteGroupAsync()
        {
            bool confirm = await _dialogService.ShowConfirmAsync("Eliminar grupo", "¿Estás seguro de que deseas eliminar este grupo?\n\nEsta acción eliminará el grupo y todos sus miembros de forma permanente.", "Eliminar", "Cancelar");
            if (!confirm)
            {
                return;
            }

            int groupId = SelectedGroupId;
            Guid sessionId = _sessionService.SessionId;

            try
            {
                if (!await SendDeleteGroupRequestAsync(groupId, sessionId))
                {
                    await _dialogService.ShowAlertAsync("No se pudo eliminar", "Ocurrió un error al intentar eliminar el grupo. Inténtalo de nuevo.", "Ok");
                    return;
                }

                await HandleDeleteGroupSuccessAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting group: {ex.Message}");
            }
        }

        private async Task<bool> SendDeleteGroupRequestAsync(int groupId, Guid sessionId)
        {
            var request = new SessionIdRequest
            {
                SessionId = sessionId
            };

            return await _groupService.DeleteGroupAsync(groupId, request);
        }

        private async Task HandleDeleteGroupSuccessAsync()
        {
            await _dialogService.ShowAlertAsync("¡Listo!", "El grupo ha sido eliminado satisfactoriamente.", "Ok");
            await BackToGroupsAsync();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task LeaveGroupAsync()
        {
            bool confirm = await _dialogService.ShowConfirmAsync("Abandonar grupo", "¿Estás seguro de que deseas abandonar este grupo? Ya no tendrás acceso a él.", "Salir", "Cancelar");
            if (!confirm)
            {
                return;
            }

            int groupId = SelectedGroupId;
            Guid sessionId = _sessionService.SessionId;

            try
            {
                if (!await SendLeaveGroupRequestAsync(groupId, sessionId))
                {
                    await _dialogService.ShowAlertAsync("No se pudo salir", "Ocurrió un error al intentar abandonar el grupo. Inténtalo de nuevo.", "Ok");
                    return;
                }

                await HandleLeaveGroupSuccessAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error leaving group: {ex.Message}");
            }
        }

        private async Task<bool> SendLeaveGroupRequestAsync(int groupId, Guid sessionId)
        {
            var request = new SessionIdRequest
            {
                SessionId = sessionId
            };

            return await _groupService.LeaveGroupAsync(groupId, request);
        }

        private async Task HandleLeaveGroupSuccessAsync()
        {
            await _dialogService.ShowAlertAsync("¡Listo!", "Has abandonado el grupo satisfactoriamente.", "Ok");
            await BackToGroupsAsync();
        }


        // ======================
        // Mappers
        // ======================

        private static List<GroupDetails> MapToGroupDetails(IEnumerable<LocalGroup> groups)
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

        private static List<GroupMember> MapToGroupMembers(IEnumerable<LocalMember> members)
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