namespace Waybon.App.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        public ProfileViewModel Profile { get; }

        private string _selectedTab = "Map";

        public string SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (SetProperty(ref _selectedTab, value))
                {
                    RaiseTabPropertiesChanged();
                }
            }
        }

        public bool IsMapVisible => SelectedTab == "Map";
        public bool IsGroupsVisible => SelectedTab == "Groups";
        public bool IsProfileVisible => SelectedTab == "Profile";

        public double MapHeight => SelectedTab == "Map" ? 78 : 68;
        public double GroupsHeight => SelectedTab == "Groups" ? 78 : 68;
        public double ProfileHeight => SelectedTab == "Profile" ? 78 : 68;

        public Color MapBackground => SelectedTab == "Map" ? Colors.White : Color.FromArgb("#F5F5F4");
        public Color MapTextColor => SelectedTab == "Map" ? Color.FromArgb("#292524") : Color.FromArgb("#57534E");

        public Color GroupsBackground => SelectedTab == "Groups" ? Colors.White : Color.FromArgb("#F5F5F4");
        public Color GroupsTextColor => SelectedTab == "Groups" ? Color.FromArgb("#292524") : Color.FromArgb("#57534E");

        public Color ProfileBackground => SelectedTab == "Profile" ? Colors.White : Color.FromArgb("#F5F5F4");
        public Color ProfileTextColor => SelectedTab == "Profile" ? Color.FromArgb("#292524") : Color.FromArgb("#57534E");

        public Command SelectMapCommand { get; }
        public Command SelectGroupsCommand { get; }
        public Command SelectProfileCommand { get; }

        public MainViewModel(ProfileViewModel profileViewModel)
        {
            Profile = profileViewModel;

            SelectMapCommand = new Command(() => SelectedTab = "Map");
            SelectGroupsCommand = new Command(() => SelectedTab = "Groups");
            SelectProfileCommand = new Command(() => SelectedTab = "Profile");
        }

        private void RaiseTabPropertiesChanged()
        {
            OnPropertyChanged(nameof(IsMapVisible));
            OnPropertyChanged(nameof(IsGroupsVisible));
            OnPropertyChanged(nameof(IsProfileVisible));

            OnPropertyChanged(nameof(MapHeight));
            OnPropertyChanged(nameof(GroupsHeight));
            OnPropertyChanged(nameof(ProfileHeight));

            OnPropertyChanged(nameof(MapBackground));
            OnPropertyChanged(nameof(MapTextColor));

            OnPropertyChanged(nameof(GroupsBackground));
            OnPropertyChanged(nameof(GroupsTextColor));

            OnPropertyChanged(nameof(ProfileBackground));
            OnPropertyChanged(nameof(ProfileTextColor));
        }
    }
}