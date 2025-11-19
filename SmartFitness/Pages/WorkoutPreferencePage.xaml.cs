namespace SmartFitness.Pages;
using Microsoft.Maui.Controls;
using SmartFitness.Authentication;
using SmartFitness.Models;
using SmartFitness.Services;
using System.Threading.Tasks;

public partial class WorkoutPreferencePage : ContentPage
{
	public WorkoutPreferencePage()
	{
		InitializeComponent();
        LoadUserData();
    }

    private async void LoadUserData()
    {
        
        try
        {
            
            var workoutPrefs = await SupabaseClient.Client
                .From<WorkoutModel>()
                .Where(w => w.UserId == App.CurrentUser.Id)
                .Single();

            if (workoutPrefs != null)
            {
                workoutLocationLabel.Text = $"{workoutPrefs.WorkoutLocation}";
                workoutTypeLabel.Text = $"{workoutPrefs.WorkoutType}";
                workoutDaysLabel.Text = $"{workoutPrefs.WorkoutDays}";
                workoutGoalLabel.Text = $"{workoutPrefs.WorkoutGoal}";
                workoutExperienceLabel.Text = $"{workoutPrefs.WorkoutExperience}";
                workoutDurationLabel.Text = $"{workoutPrefs.WorkoutDuration} minutes";
            }
    
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load account data: {ex.Message}", "OK");
        }
    }

    private async void OnBackButton(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}