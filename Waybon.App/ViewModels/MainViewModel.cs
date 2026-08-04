namespace Waybon.App.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        public ProfileViewModel Profile
        {
            get;
        }

        private string _selectedTab = "Map";

        public string SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (SetProperty(ref _selectedTab, value))
                {
                    TabChaged();
                }
            }
        }

        public bool IsMapVisible => SelectedTab == "Map";
        public bool IsGroupsVisible => SelectedTab == "Groups";
        public bool IsProfileVisible => SelectedTab == "Profile";

        public Color MapButtonBackground => IsMapVisible ? Color.FromArgb("#292524") : Colors.Transparent;
        public Color MapButtonTextColor => IsMapVisible ? Colors.White : Color.FromArgb("#57534E");

        public Color GroupsButtonBackground => IsGroupsVisible ? Color.FromArgb("#292524") : Colors.Transparent;
        public Color GroupsButtonTextColor => IsGroupsVisible ? Colors.White : Color.FromArgb("#57534E");

        public Color ProfileButtonBackground => IsProfileVisible ? Color.FromArgb("#292524") : Colors.Transparent;
        public Color ProfileButtonTextColor => IsProfileVisible ? Colors.White : Color.FromArgb("#57534E");

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

        private void TabChaged()
        {
            OnPropertyChanged(nameof(IsMapVisible));
            OnPropertyChanged(nameof(IsGroupsVisible));
            OnPropertyChanged(nameof(IsProfileVisible));

            OnPropertyChanged(nameof(MapButtonBackground));
            OnPropertyChanged(nameof(MapButtonTextColor));
            OnPropertyChanged(nameof(GroupsButtonBackground));
            OnPropertyChanged(nameof(GroupsButtonTextColor));
            OnPropertyChanged(nameof(ProfileButtonBackground));
            OnPropertyChanged(nameof(ProfileButtonTextColor));
        }
    }
}