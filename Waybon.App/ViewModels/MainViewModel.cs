using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Waybon.App.ViewModels
{
    public partial class MainViewModel(ProfileViewModel profileViewModel, GroupViewModel groupViewModel) : ObservableObject
    {
        public ProfileViewModel Profile { get; } = profileViewModel;
        public GroupViewModel Group { get; } = groupViewModel;


        // ======================
        // Properties
        // ======================

        [ObservableProperty]
        public partial WebViewSource? MapSource { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsMapVisible))]
        [NotifyPropertyChangedFor(nameof(IsGroupsVisible))]
        [NotifyPropertyChangedFor(nameof(IsProfileVisible))]
        [NotifyPropertyChangedFor(nameof(MapButtonBackground))]
        [NotifyPropertyChangedFor(nameof(MapButtonTextColor))]
        [NotifyPropertyChangedFor(nameof(GroupsButtonBackground))]
        [NotifyPropertyChangedFor(nameof(GroupsButtonTextColor))]
        [NotifyPropertyChangedFor(nameof(ProfileButtonBackground))]
        [NotifyPropertyChangedFor(nameof(ProfileButtonTextColor))]
        public partial string SelectedTab { get; set; } = "Map";


        // ======================
        // Visibility
        // ======================

        public bool IsMapVisible => SelectedTab == "Map";
        public bool IsGroupsVisible => SelectedTab == "Groups";
        public bool IsProfileVisible => SelectedTab == "Profile";


        // ======================
        // Tab Styling
        // ======================

        public Color MapButtonBackground => IsMapVisible ? Color.FromArgb("#292524") : Colors.Transparent;
        public Color MapButtonTextColor => IsMapVisible ? Colors.White : Color.FromArgb("#57534E");

        public Color GroupsButtonBackground => IsGroupsVisible ? Color.FromArgb("#292524") : Colors.Transparent;
        public Color GroupsButtonTextColor => IsGroupsVisible ? Colors.White : Color.FromArgb("#57534E");

        public Color ProfileButtonBackground => IsProfileVisible ? Color.FromArgb("#292524") : Colors.Transparent;
        public Color ProfileButtonTextColor => IsProfileVisible ? Colors.White : Color.FromArgb("#57534E");


        // ======================
        // Commands
        // ======================

        [RelayCommand]
        private void SelectMap() => SelectedTab = "Map";

        [RelayCommand]
        private void SelectGroups()
        {
            SelectedTab = "Groups";
            _ = Group.RestoreLastStateAsync();
        }

        [RelayCommand]
        private void SelectProfile() => SelectedTab = "Profile";

        [RelayCommand]
        private async Task InitializeMainAsync() => await LoadMapAsync();


        // ======================
        // Initialization
        // ======================

        public async Task LoadMapAsync()
        {
            try
            {
                var html = await ReadResourceAsync("map.html");
                var css = await ReadResourceAsync("maplibre-gl.css");
                var js = await ReadResourceAsync("maplibre-gl.js");
                var styleJson = await ReadResourceAsync("style.json");

                html = html.Replace("<link href=\"maplibre-gl.css\" rel=\"stylesheet\" />", $"<style>{css}</style>");
                html = html.Replace("<script src=\"maplibre-gl.js\"></script>", $"<script>{js}</script>");
                html = html.Replace("MAP_STYLE_JSON_PLACEHOLDER", styleJson);

                MapSource = new HtmlWebViewSource
                {
                    Html = html,
                    BaseUrl = "https://localhost"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading map: {ex.Message}");
                MapSource = new HtmlWebViewSource
                {
                    Html = $"<html><body style='font-family:Georgia;padding:20px;color:#78716c;'><h1>Error</h1><p>{ex.Message}</p></body></html>"
                };
            }
        }

        private static async Task<string> ReadResourceAsync(string filename)
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(filename);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}