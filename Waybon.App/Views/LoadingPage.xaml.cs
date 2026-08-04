using Waybon.App.ViewModels;

namespace Waybon.App.Views
{
    public partial class LoadingPage : ContentPage
    {
        public LoadingPage(LoadingViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}