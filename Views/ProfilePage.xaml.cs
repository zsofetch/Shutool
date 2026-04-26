using Shutool.ViewModels;

namespace Shutool.Views
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage(ProfileViewModel viewModel)
        {
            //InitializeComponent();
            BindingContext = viewModel;
        }
    }
}