namespace Shutool.Views; // NO "public" here!

public partial class AppShell : Shell
{
    public AppShell()
    {
        //InitializeComponent();
        Routing.RegisterRoute("SignUpPage", typeof(SignUpPage));

        Routing.RegisterRoute("RiderHistoryPage", typeof(RiderHistoryPage));

        Routing.RegisterRoute("DriverHistoryPage", typeof(DriverHistoryPage));

        Routing.RegisterRoute("ProfilePage", typeof(Views.ProfilePage));

        Routing.RegisterRoute("AdminRequestLogPage", typeof(Views.AdminRequestLogPage));
    }
}

//test