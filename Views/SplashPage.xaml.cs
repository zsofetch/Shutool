using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace Shutool.Views
{
    public partial class SplashPage : ContentPage
    {
        public SplashPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Wait for 2.5 seconds to let the user see the "Looking for a ride..." animation/text
            await Task.Delay(2500);

            // Navigate securely to the Login Page using absolute routing (//) 
            // This prevents the user from hitting the "Back" button to return to the splash screen.
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}