using Shutool.ViewModels;

namespace Shutool.Views
{
    public partial class AdminRequestLogPage : ContentPage
    {
        public AdminRequestLogPage(AdminRequestLogViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}