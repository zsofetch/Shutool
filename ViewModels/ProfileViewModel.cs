using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shutool.Models;
using Shutool.Services;

namespace Shutool.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly SupabaseService _supabaseService;

    [ObservableProperty] private string username;
    [ObservableProperty] private string email;
    [ObservableProperty] private string studentId;

    // We bind the Image Source in XAML directly to this property
    [ObservableProperty] private string currentAvatarImage;
    private int _currentAvatarId = 1;

    public ProfileViewModel(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
        LoadProfileAsync();
    }

    private async Task LoadProfileAsync()
    {
        var profile = await _supabaseService.GetCurrentUserProfileAsync();
        if (profile != null)
        {
            Username = profile.Username;
            Email = profile.Email; // Read-only in UI
            StudentId = profile.StudentId;
            _currentAvatarId = profile.AvatarId == 0 ? 1 : profile.AvatarId;

            UpdateAvatarImage();
        }
    }

    [RelayCommand]
    private void ChangeAvatar()
    {
        // Cycles through 4 pre-determined avatars (adjust the number based on how many PNGs you have)
        _currentAvatarId++;
        if (_currentAvatarId > 4) _currentAvatarId = 1;

        UpdateAvatarImage();
    }

    private void UpdateAvatarImage()
    {
        // Assuming your files are named avatar_1.png, avatar_2.png, etc.
        CurrentAvatarImage = $"avatar_{_currentAvatarId}.png";
    }

    [RelayCommand]
    private async Task SaveProfileAsync()
    {
        var success = await _supabaseService.UpdateProfileAsync(Username, StudentId, _currentAvatarId);

        if (success)
            await Application.Current.MainPage.DisplayAlert("Success", "Profile updated!", "OK");
        else
            await Application.Current.MainPage.DisplayAlert("Error", "Could not save profile.", "OK");
    }
}