using Shutool.ViewModels;

namespace Shutool.Views
{
    public partial class RiderHistoryPage : ContentPage
    {
        public RiderHistoryPage(RiderHistoryViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}