using Shutool.ViewModels;

namespace Shutool.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();

            // This links the UI to the logic and commands in the ViewModel
            BindingContext = viewModel;
        }
    }
}