using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shutool.Models;
using Shutool.Services;

namespace Shutool.ViewModels;

public partial class RiderHistoryViewModel : ObservableObject
{
    private readonly SupabaseService _supabaseService;

    public ObservableCollection<RequestModel> RequestHistory { get; } = new();

    public RiderHistoryViewModel(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
        LoadHistoryAsync();
    }

    [RelayCommand]
    private async Task LoadHistoryAsync()
    {
        var history = await _supabaseService.GetRiderHistoryAsync();

        RequestHistory.Clear();
        foreach (var req in history)
        {
            RequestHistory.Add(req);
        }
    }

    [RelayCommand]
    private async Task DeleteRequestAsync(RequestModel request)
    {
        if (request == null) return;

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Delete Request",
            "Are you sure you want to remove this from your history?",
            "Yes", "No");

        if (confirm)
        {
            var success = await _supabaseService.DeleteRequestAsync(request.Id);
            if (success)
            {
                RequestHistory.Remove(request); // Instantly updates the UI
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete request.", "OK");
            }
        }
    }

    [RelayCommand]
    private async Task GoBackToPriorityAsync()
    {
        await Shell.Current.GoToAsync(".."); // Navigates back to the main request page
    }

    [RelayCommand]
    private async Task GoToProfileAsync()
    {
        // Uses absolute routing to push the profile page on top of the current stack
        await Shell.Current.GoToAsync("ProfilePage");
    }
}