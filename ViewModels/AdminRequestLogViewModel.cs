using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shutool.Models;
using Shutool.Services;

namespace Shutool.ViewModels;

public partial class AdminRequestLogViewModel : ObservableObject
{
    private readonly SupabaseService _supabaseService;
    private List<AdminRequestDisplayModel> _allLogs = new();

    public ObservableCollection<AdminRequestDisplayModel> DisplayLogs { get; } = new();

    [ObservableProperty] private string searchText;
    [ObservableProperty] private string selectedStatus = "All";

    public List<string> StatusOptions { get; } = new() { "All", "pending", "completed" };

    public AdminRequestLogViewModel(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
        LoadLogsAsync();
    }

    [RelayCommand]
    private async Task LoadLogsAsync()
    {
        _allLogs = await _supabaseService.GetAllRequestsLogAsync();
        ApplyFilters();
    }

    partial void OnSearchTextChanged(string value) => ApplyFilters();
    partial void OnSelectedStatusChanged(string value) => ApplyFilters();

    private void ApplyFilters()
    {
        var filtered = _allLogs.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(l => l.RequestedBy.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                           l.ShortId.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        if (SelectedStatus != "All")
        {
            filtered = filtered.Where(l => l.Status == SelectedStatus);
        }

        DisplayLogs.Clear();
        foreach (var log in filtered)
        {
            DisplayLogs.Add(log);
        }
    }

    [RelayCommand]
    private async Task GoToManageUsersAsync()
    {
        await Shell.Current.GoToAsync(".."); // Go back to the Manage Users page
    }
}