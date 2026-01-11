using SmartFitness.ViewModels;
using SmartFitness.Models;
using SmartFitness.Services;
using Supabase.Postgrest; 

namespace SmartFitness.Pages;

public partial class WorkoutDetailPage : ContentPage
{
    private bool _isSaved = false;
    private string _savedRecordId = null;

    public WorkoutDetailPage(WorkoutDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        
        CheckIfSaved();
    }


    private async void CheckIfSaved()
    {
        if (App.CurrentUser == null) return;

        var viewModel = BindingContext as WorkoutDetailViewModel;
        if (viewModel?.Workout == null) return;

        try
        {
            var response = await SupabaseClient.Client
                .From<SavedWorkout>()
                .Where(x => x.UserId == App.CurrentUser.Id && x.WorkoutId == viewModel.Workout.Id)
                .Single();

            if (response != null)
            {
                _isSaved = true;
                _savedRecordId = response.Id;
                UpdateIcon();
            }
        }
        catch
        {
            // Nincs elmentve
            _isSaved = false;
            UpdateIcon();
        }
    }

    private void UpdateIcon()
    {
        SaveButton.Source = _isSaved ? "bookmark_filled_icon.svg" : "bookmark_unfilled_icon.svg";
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (App.CurrentUser == null)
        {
            await DisplayAlert("Error", "Please log in to save workouts.", "OK");
            return;
        }

        var viewModel = BindingContext as WorkoutDetailViewModel;
        if (viewModel?.Workout == null) return;

        SaveButton.IsEnabled = false; 

        try
        {
            if (_isSaved)
            {
                
                await SupabaseClient.Client
                    .From<SavedWorkout>()
                    .Where(x => x.Id == _savedRecordId)
                    .Delete();

                _isSaved = false;
                _savedRecordId = null;
               // await DisplayAlert("Success", "Workout removed from saved.", "OK");
            }
            else
            {
                
                var newSave = new SavedWorkout
                {
                    UserId = App.CurrentUser.Id,
                    WorkoutId = viewModel.Workout.Id
                };

                var response = await SupabaseClient.Client
                    .From<SavedWorkout>()
                    .Insert(newSave);

                _isSaved = true;
                _savedRecordId = response.Models.FirstOrDefault()?.Id;

                //await DisplayAlert("Success", "Workout saved to bookmarks!", "OK");
            }
            UpdateIcon();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Operation failed: {ex.Message}", "OK");
        }
        finally
        {
            SaveButton.IsEnabled = true;
        }
    }

 

    private async void WorkoutSaveButton_Clicked(object sender, EventArgs e)
    {
        var vm = BindingContext as WorkoutDetailViewModel;

        if (vm == null || vm.Workout == null)
            return;

        string workoutName = vm.Workout.Title;

        try
        {
            await WorkoutService.SaveWorkoutAsync(App.CurrentUser.Id, workoutName);

            // Streak frissítése
            var streak = await WorkoutStreakService.GetStreakAsync(App.CurrentUser.Id);

            //await DisplayAlert("Great Job!", $"Workout logged successfully!", "OK");

            // Visszalépés a fõoldalra sikeres edzés után
             await Navigation.PopAsync(); 
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void OnBackButton(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}