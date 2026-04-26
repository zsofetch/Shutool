using Shutool.ViewModels;

namespace Shutool.Views
{
    public partial class RiderMainPage : ContentPage
    {
        public RiderMainPage(RiderMainViewModel viewModel)
        {
            InitializeComponent();

            // This links the UI elements to the commands and variables in the ViewModel
            BindingContext = viewModel;
        }
    }
}