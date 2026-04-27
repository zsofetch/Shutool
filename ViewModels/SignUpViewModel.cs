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
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current.MainPage.DisplayAlert("Oops", "Please fill in all fields.", "OK");
            return;
        }

        if (Password != ConfirmPassword)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Passwords do not match.", "OK");
            return;
        }

        // Call the updated method
        var result = await _supabaseService.SignUpUserAsync(Email, Password, Name);

        if (result.IsSuccess)
        {
            await Application.Current.MainPage.DisplayAlert("Success", "Account created! Please log in.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            // Now it displays the actual error from the database!
            await Application.Current.MainPage.DisplayAlert("Failed", result.Message, "OK");
        }
    }
}