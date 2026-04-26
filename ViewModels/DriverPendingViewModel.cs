using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shutool.Models;
using Shutool.Services;

namespace Shutool.ViewModels;

public partial class DriverPendingViewModel : ObservableObject
{
    private readonly SupabaseService _supabaseService;

    // This collection binds directly to your CollectionView in the UI
    public ObservableCollection<DriverRequestDisplayModel> PendingRequests { get; } = new();

    public DriverPendingViewModel(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
        LoadRequestsAsync(); // Load automatically when the ViewModel is created
    }

    [RelayCommand]
    private async Task LoadRequestsAsync()
    {
        var requests = await _supabaseService.GetPendingRequestsAsync();

        PendingRequests.Clear();
        foreach (var req in requests)
        {
            PendingRequests.Add(req);
        }
    }

    [RelayCommand]
    private async Task GoToHistoryAsync()
    {
        await Shell.Current.GoToAsync("DriverHistoryPage");
    }

    [RelayCommand]
    private async Task CompleteRideAsync(DriverRequestDisplayModel request)
    {
        if (request == null) return;

        var success = await _supabaseService.CompleteRequestAsync(request.RequestId);

        if (success)
        {
            // Instantly remove it from the pending UI list
            PendingRequests.Remove(request);
            await Application.Current.MainPage.DisplayAlert("Success", $"{request.RiderName} has been picked up!", "OK");
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Failed to update request.", "OK");
        }
    }

    [RelayCommand]
    private async Task GoToProfileAsync()
    {
        // Uses absolute routing to push the profile page on top of the current stack
        await Shell.Current.GoToAsync("ProfilePage");
    }
}

//(Note: To use this in your UI, you would wrap the DataTemplate in your DriverPendingPage.xaml
//inside a SwipeView, exactly like we did for the Rider's delete function, but binding it to CompleteRideCommand with a green background!)