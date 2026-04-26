using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shutool.Services;

namespace Shutool.ViewModels;

public partial class SignUpViewModel : ObservableObject
{
    private readonly SupabaseService _supabaseService;

    [ObservableProperty] private string name;
    [ObservableProperty] private string email;
    [ObservableProperty] private string password;
    [ObservableProperty] private string confirmPassword;

    public SignUpViewModel(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    [RelayCommand]
    private async Task SignUpAsync()
    {
        if (Password != ConfirmPassword)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Passwords do not match.", "OK");
            return;
        }

        var success = await _supabaseService.SignUpUserAsync(Email, Password, Name);

        if (success)
        {
            await Application.Current.MainPage.DisplayAlert("Success", "Account created! Please log in.", "OK");
            await Shell.Current.GoToAsync(".."); // Go back to login
        }
    }
}