using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shutool.Models;
using Shutool.Services;

namespace Shutool.ViewModels;

public partial class DriverPendingViewModel : ObservableObject
{
    private readonly SupabaseService _supabaseService;

    public ObservableCollection<DriverRequestDisplayModel> PendingRequests { get; } = new();

    [ObservableProperty] private string driverName = "Loading...";
    [ObservableProperty] private string shuttleDisplay = "Shuttle: --";

    public DriverPendingViewModel(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
        LoadRequestsAsync();
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
    private async Task LoadRequestsAsync()
    {
        await LoadProfileAsync();

        var requests = await _supabaseService.GetPendingRequestsAsync();
        PendingRequests.Clear();
        foreach (var req in requests)
        {
            PendingRequests.Add(req);
        }
    }

    [RelayCommand]
    private async Task MarkAllCompleteAsync()
    {
        await _supabaseService.MarkAllPendingAsCompleteAsync();
        await LoadRequestsAsync();
        await Application.Current.MainPage.DisplayAlert("Success", "All requests marked as complete!", "OK");
    }

    [RelayCommand]
    private async Task CompleteRideAsync(DriverRequestDisplayModel request)
    {
        if (request == null) return;

        // 1. Changed "request.Id" to "request.RequestId"
        // 2. Changed "MarkRequestCompletedAsync" to "CompleteRequestAsync"
        var success = await _supabaseService.CompleteRequestAsync(request.RequestId);

        if (success)
        {
            await LoadRequestsAsync();
        }
    }

    [RelayCommand]
    private async Task GoToHistoryAsync()
    {
        await Shell.Current.GoToAsync("DriverHistoryPage");
    }

    [RelayCommand]
    private async Task GoToProfileAsync()
    {
        await Shell.Current.GoToAsync("ProfilePage");
    }
}



//(Note: To use this in your UI, you would wrap the DataTemplate in your DriverPendingPage.xaml
//inside a SwipeView, exactly like we did for the Rider's delete function, but binding it to CompleteRideCommand with a green background!)