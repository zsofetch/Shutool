using Shutool.ViewModels;

namespace Shutool.Views
{
    public partial class DriverHistoryPage : ContentPage
    {
        public DriverHistoryPage(DriverHistoryViewModel viewModel)
        {
            //InitializeComponent();
            BindingContext = viewModel;
        }
    }
}