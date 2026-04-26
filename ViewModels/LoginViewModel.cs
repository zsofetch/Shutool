using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shutool.Services;

namespace Shutool.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly SupabaseService _supabaseService;

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string password;

    public LoginViewModel(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            return;

        var role = await _supabaseService.LoginUserAsync(Email, Password);

        // Route based on role
        if (role == "rider")
            await Shell.Current.GoToAsync("//RiderHome");
        else if (role == "driver")
            await Shell.Current.GoToAsync("//DriverPending");
        else if (role == "admin")
            await Shell.Current.GoToAsync("//AdminManage");
        else
            await Application.Current.MainPage.DisplayAlert("Error", "Invalid credentials", "OK");
    }

    [RelayCommand]
    private async Task GoToSignUpAsync()
    {
        await Shell.Current.GoToAsync("SignUpPage");
    }
}