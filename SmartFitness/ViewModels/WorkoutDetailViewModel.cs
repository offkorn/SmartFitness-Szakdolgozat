using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Supabase;
using SmartFitness.Models;
using SmartFitness.Services;

namespace SmartFitness.ViewModels;

public partial class WorkoutDetailViewModel : ObservableObject
{
    private readonly Client _supabase = SupabaseClient.Client;

    [ObservableProperty]
    private Workout workout;

    public WorkoutDetailViewModel()
    {
    }

    public async Task LoadWorkoutAsync(string workoutId)
    {
        try
        {
            var response = await _supabase.From<Workout>()
                .Where(w => w.Id == workoutId)
                .Single();

            if (response != null)
            {
                Workout = response;
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Hiba", "Az edzés nem található.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Hiba", $"Nem sikerült betölteni az edzést: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task SaveWorkout()
    {
        // Mentési logika későbbi továbbfejlesztéshez
        //await Application.Current.MainPage.DisplayAlert("Info", "Az edzés mentése még nem implementálva.", "OK");
    }
}