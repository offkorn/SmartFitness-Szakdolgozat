namespace SmartFitness.Pages;
using Microsoft.Maui.Controls;
using SmartFitness.Models;
using SmartFitness.Services;


public partial class DietPreferencePage : ContentPage
{
	public DietPreferencePage()
	{
		InitializeComponent();
        LoadUserData();
	}

    private async void LoadUserData()
    {


        try
        {
            
            var dietPrefs = await SupabaseClient.Client
                .From<DietPreferences>()
                .Where(d => d.UserId == App.CurrentUser.Id)
                .Single();

            if (dietPrefs != null)
            {
                intoleranceLabel.Text = $"{dietPrefs.FoodIntolerance}";
                dislikedFoodLabel.Text = $"{dietPrefs.DislikedFoods}";
                dietGoalLabel.Text = $"{dietPrefs.DietGoal}";
                cookingTimeLabel.Text = $"{dietPrefs.CookingTime} minutes";
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