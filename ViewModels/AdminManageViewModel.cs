using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shutool.Models;
using Shutool.Services;

namespace Shutool.ViewModels;

public partial class AdminManageViewModel : ObservableObject
{
    private readonly SupabaseService _supabaseService;
    private List<UserModel> _allUsers = new(); // The master list

    public ObservableCollection<UserModel> DisplayUsers { get; } = new(); // The filtered UI list

    [ObservableProperty] private string searchText;
    [ObservableProperty] private string selectedRole = "All";

    public List<string> Roles { get; } = new() { "All", "rider", "driver", "admin" };

    public AdminManageViewModel(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
        LoadUsersAsync();
    }

    [RelayCommand]
    private async Task LoadUsersAsync()
    {
        _allUsers = await _supabaseService.GetAllUsersAsync();
        ApplyFilters();
    }

    // Trigger this automatically when SearchText changes
    partial void OnSearchTextChanged(string value) => ApplyFilters();

    // Trigger this automatically when SelectedRole changes
    partial void OnSelectedRoleChanged(string value) => ApplyFilters();

    private void ApplyFilters()
    {
        var filtered = _allUsers.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(u => u.Username != null &&
                                           u.Username.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        if (SelectedRole != "All")
        {
            filtered = filtered.Where(u => u.Role == SelectedRole);
        }

        DisplayUsers.Clear();
        foreach (var user in filtered)
        {
            DisplayUsers.Add(user);
        }
    }

    [RelayCommand]
    private async Task GoToRequestLogAsync()
    {
        await Shell.Current.GoToAsync("AdminRequestLogPage");
    }
}