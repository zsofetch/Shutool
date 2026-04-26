namespace Shutool;
using Shutool.Views;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes using the proper namespace, with 'using Shutool.Views' added
        Routing.RegisterRoute("SignUpPage", typeof(SignUpPage));
        Routing.RegisterRoute("RiderHistoryPage", typeof(RiderHistoryPage));
        Routing.RegisterRoute("DriverHistoryPage", typeof(DriverHistoryPage));
        Routing.RegisterRoute("ProfilePage", typeof(ProfilePage)); // <-- No need to prefix with Views
        Routing.RegisterRoute("AdminRequestLogPage", typeof(AdminRequestLogPage)); // <-- No need to prefix with Views
    }
}