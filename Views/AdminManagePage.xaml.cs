using Shutool.ViewModels;

namespace Shutool.Views
{
    public partial class AdminManagePage : ContentPage
    {
        public AdminManagePage(AdminManageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}