using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shutool.Services;

namespace Shutool.ViewModels;

public partial class RiderMainViewModel : ObservableObject
{
    private readonly SupabaseService _supabaseService;

    [ObservableProperty]
    private string reasonText;

    public RiderMainViewModel(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    [RelayCommand]
    private async Task SendRequestAsync()
    {
        if (string.IsNullOrWhiteSpace(ReasonText))
        {
            await Application.Current.MainPage.DisplayAlert("Oops", "Please state why you need a ride.", "OK");
            return;
        }

        var result = await _supabaseService.CreatePriorityRequestAsync(ReasonText);

        if (result.IsSuccess)
        {
            await Application.Current.MainPage.DisplayAlert("Success", result.Message, "OK");
            ReasonText = string.Empty; // Clear the text box after sending
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Limit Reached", result.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task GoToHistoryAsync()
    {
        await Shell.Current.GoToAsync("RiderHistoryPage");
    }

    [RelayCommand]
    private async Task GoToProfileAsync()
    {
        // Uses absolute routing to push the profile page on top of the current stack
        await Shell.Current.GoToAsync("ProfilePage");
    }
}