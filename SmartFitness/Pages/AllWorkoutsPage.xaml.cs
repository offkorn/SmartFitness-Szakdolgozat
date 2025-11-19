using SmartFitness.Models;
using Supabase;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using SmartFitness.ViewModels;

namespace SmartFitness.Pages;

public partial class AllWorkoutsPage : ContentPage
{
    private readonly Client _supabase;
    private ObservableCollection<Workout> Workouts { get; } = new();
    public ICommand ViewWorkoutCommand { get; }

    public AllWorkoutsPage(Client supabase)
    {
        InitializeComponent();
        _supabase = supabase;
        BindingContext = this;
        ViewWorkoutCommand = new Command<Workout>(async (workout) => await ViewWorkout(workout));
        LoadWorkouts();
    }

    private async void LoadWorkouts()
    {
        try
        {


            if (!Guid.TryParse(App.CurrentUser.Id, out _))
            {
                Debug.WriteLine($"Hiba: Érvénytelen UUID formátum: {App.CurrentUser.Id}");
                await DisplayAlert("Hiba", "Érvénytelen felhasználói azonosító!", "OK");
                
                return;
            }

         
            var workoutResponse = await _supabase.From<Workout>()
                .Get();

            Workouts.Clear();
            foreach (var workout in workoutResponse.Models)
            {
                Workouts.Add(workout);
            }

            WorkoutsCollection.ItemsSource = Workouts;
            Debug.WriteLine($"AllWorkoutsPage: {Workouts.Count} SimpleWorkout betöltve");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"LoadWorkouts hiba: {ex.Message}\nStackTrace: {ex.StackTrace}");
            await DisplayAlert("Hiba", $"Nem sikerült betölteni az edzéseket: {ex.Message}", "OK");
        }
    }

    private async Task ViewWorkout(Workout workout)
    {
        if (workout != null)
        {
            var viewModel = new WorkoutDetailViewModel();
            await viewModel.LoadWorkoutAsync(workout.Id);
            await Navigation.PushAsync(new WorkoutDetailPage(viewModel));
        }
    }
}