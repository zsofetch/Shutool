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

    public DriverHistoryViewModel(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
        LoadHistoryAsync();
    }

    [RelayCommand]
    private async Task LoadHistoryAsync()
    {
        var history = await _supabaseService.GetDriverHistoryAsync();

        HistoryRequests.Clear();
        foreach (var req in history)
        {
            HistoryRequests.Add(req);
        }
    }

    [RelayCommand]
    private async Task ClearHistoryAsync()
    {
        // Optional: Add a call to SupabaseService to physically delete these if you want, 
        // or just clear them locally from the UI.
        HistoryRequests.Clear();
    }

    [RelayCommand]
    private async Task GoBackToPendingAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task GoToProfileAsync()
    {
        // Uses absolute routing to push the profile page on top of the current stack
        await Shell.Current.GoToAsync("ProfilePage");
    }
}