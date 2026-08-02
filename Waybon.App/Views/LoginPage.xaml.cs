using System.Net.Http.Json;
using Waybon.App.Models;
using Waybon.App.ViewModels;

namespace Waybon.App.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}