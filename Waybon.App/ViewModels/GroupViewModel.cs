using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text.Json;
using Waybon.App.Models;
using Waybon.App.Services;
using Waybon.App.Services.Interfaces;

namespace Waybon.App.ViewModels
{
    public partial class GroupViewModel(IGroupService groupService, IPreferencesService preferencesService) : ObservableObject
    {
        private readonly IGroupService _groupService = groupService;
        private readonly IPreferencesService _preferencesService = preferencesService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasGroups))]
        [NotifyPropertyChangedFor(nameof(IsEmpty))]
        public partial ObservableCollection<GroupDetails> Groups { get; set; } = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEmpty))]
        public partial bool IsLoading { get; set; } = true;

        public bool HasGroups
        {
            get
            {
                if (Groups.Count > 0)
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsEmpty
        {
            get
            {
                if (Groups.Count == 0 && !IsLoading)
                {
                    return true;
                }

                return false;
            }
        }

        [RelayCommand]
        public async Task LoadGroupsAsync()
        {
            IsLoading = true;
            Groups.Clear();

            try
            {
                var sessionIdStr = _preferencesService.Get("waybon_sessionId");
                if (string.IsNullOrEmpty(sessionIdStr) || !Guid.TryParse(sessionIdStr, out var sessionId))
                {
                    return;
                }

                var request = new SessionIdRequest
                {
                    SessionId = sessionId
                };

                var groups = await _groupService.GetJoinedGroupsAsync(request);
                _preferencesService.Set("waybon_groups", JsonSerializer.Serialize(groups));

                var currentUserId = _preferencesService.Get("waybon_userId");

                var tempGroups = new List<GroupDetails>();
                foreach (var g in groups)
                {
                    var displayUsername = g.Username;
                    if (g.OwnerUserId.ToString() == currentUserId)
                    {
                        displayUsername = $"{g.Username} (Yo)";
                    }

                    tempGroups.Add(new GroupDetails
                    {
                        GroupId = g.GroupId,
                        OwnerUserId = g.OwnerUserId,
                        Username = displayUsername,
                        Name = g.Name,
                        JoinCode = g.JoinCode,
                        JoinCodeExpiresAt = g.JoinCodeExpiresAt,
                        CreatedAt = g.CreatedAt
                    });
                }

                Groups = new ObservableCollection<GroupDetails>(tempGroups);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando grupos: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}