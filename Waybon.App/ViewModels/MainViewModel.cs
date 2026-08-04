using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Waybon.App.ViewModels
{
    public partial class MainViewModel(ProfileViewModel profileViewModel, GroupViewModel groupViewModel) : ObservableObject
    {
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

        public ProfileViewModel Profile { get; } = profileViewModel;
        public GroupViewModel Group { get; } = groupViewModel;

        public bool IsMapVisible
        {
            get
            {
                if (SelectedTab == "Map")
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsGroupsVisible
        {
            get
            {
                if (SelectedTab == "Groups")
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsProfileVisible
        {
            get
            {
                if (SelectedTab == "Profile")
                {
                    return true;
                }

                return false;
            }
        }

        public Color MapButtonBackground
        {
            get
            {
                if (IsMapVisible)
                {
                    return Color.FromArgb("#292524");
                }

                return Colors.Transparent;
            }
        }

        public Color MapButtonTextColor
        {
            get
            {
                if (IsMapVisible)
                {
                    return Colors.White;
                }

                return Color.FromArgb("#57534E");
            }
        }

        public Color GroupsButtonBackground
        {
            get
            {
                if (IsGroupsVisible)
                {
                    return Color.FromArgb("#292524");
                }

                return Colors.Transparent;
            }
        }

        public Color GroupsButtonTextColor
        {
            get
            {
                if (IsGroupsVisible)
                {
                    return Colors.White;
                }

                return Color.FromArgb("#57534E");
            }
        }

        public Color ProfileButtonBackground
        {
            get
            {
                if (IsProfileVisible)
                {
                    return Color.FromArgb("#292524");
                }

                return Colors.Transparent;
            }
        }

        public Color ProfileButtonTextColor
        {
            get
            {
                if (IsProfileVisible)
                {
                    return Colors.White;
                }

                return Color.FromArgb("#57534E");
            }
        }

        [RelayCommand]
        private void SelectMap()
        {
            SelectedTab = "Map";
        }

        [RelayCommand]
        private async Task SelectGroupsAsync()
        {
            SelectedTab = "Groups";
            await Group.LoadGroupsAsync();
        }

        [RelayCommand]
        private void SelectProfile()
        {
            SelectedTab = "Profile";
        }

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
                MapSource = new HtmlWebViewSource
                {
                    Html = $"<html><body style='font-family:Georgia;padding:20px;color:#78716c;'><h1>Error</h1><p>{ex.Message}</p></body></html>"
                };
            }
        }

        [RelayCommand]
        public async Task InitializeMainAsync()
        {
            await LoadMapAsync();
        }

        private static async Task<string> ReadResourceAsync(string filename)
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(filename);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}