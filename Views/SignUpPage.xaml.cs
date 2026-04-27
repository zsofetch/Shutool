using Shutool.ViewModels;

namespace Shutool.Views
{
    public partial class SignUpPage : ContentPage
    {
        public SignUpPage(SignUpViewModel viewModel)
        {
            InitializeComponent();

            // This is the crucial line! It tells the UI to use your ViewModel for all {Binding ...} commands.
            BindingContext = viewModel;
        }
    }
}