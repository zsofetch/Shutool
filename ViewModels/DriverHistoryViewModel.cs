using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shutool.Models;
using Shutool.Services;

namespace Shutool.ViewModels;

public partial class DriverHistoryViewModel : ObservableObject
{
    private readonly SupabaseService _supabaseService;

    public ObservableCollection<DriverRequestDisplayModel> HistoryRequests { get; } = new();

    [ObservableProperty] private string driverName = "Loading...";
    [ObservableProperty] private string shuttleDisplay = "Shuttle: --";

    public DriverHistoryViewModel(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
        LoadHistoryAsync();
    }

    private async Task LoadProfileAsync()
    {
        var profile = await _supabaseService.GetCurrentUserProfileAsync();
        if (profile != null)
        {
            DriverName = profile.Username ?? "Unknown Driver";
            ShuttleDisplay = profile.ShuttleNumber != null ? $"Shuttle {profile.ShuttleNumber}" : "Unassigned";
        }
    }

    [RelayCommand]
    private async Task LoadHistoryAsync()
    {
        await LoadProfileAsync();

        var requests = await _supabaseService.GetDriverHistoryAsync();
        HistoryRequests.Clear();
        foreach (var req in requests)
        {
            HistoryRequests.Add(req);
        }
    }

    [RelayCommand]
    private async Task GoBackToPendingAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task ClearHistoryAsync()
    {
        bool answer = await Application.Current.MainPage.DisplayAlert("Confirm", "Clear your history?", "Yes", "No");
        if (answer)
        {
            // Optional: Implement actual delete logic in SupabaseService here if desired.
            HistoryRequests.Clear();
        }
    }

    [RelayCommand]
    private async Task GoToProfileAsync()
    {
        await Shell.Current.GoToAsync("ProfilePage");
    }
}