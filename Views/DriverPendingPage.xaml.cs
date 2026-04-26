using Shutool.ViewModels;

namespace Shutool.Views
{
    public partial class DriverPendingPage : ContentPage
    {
        public DriverPendingPage(DriverPendingViewModel viewModel)
        {
            InitializeComponent();

            // Link the UI elements (like the CollectionView) to the data in the ViewModel
            BindingContext = viewModel;
        }
    }
}