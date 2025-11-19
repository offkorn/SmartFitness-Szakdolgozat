using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartFitness.Models;
using SmartFitness.Pages;
using Supabase;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SmartFitness.ViewModels;

public partial class WorkoutPlanDetailViewModel : ObservableObject
{
    private readonly Client _supabase;

    [ObservableProperty]
    private WorkoutPlan workoutPlan;

    [ObservableProperty]
    private ObservableCollection<PlanDay> planDays = new();

    [ObservableProperty]
    private bool isProgramStarted;

    public WorkoutPlanDetailViewModel(Client supabase)
    {
        _supabase = supabase;
        IsProgramStarted = false; // Alapértelmezetten a Start gomb látható
    }

    public async Task LoadPlanAsync(string planId)
    {
        try
        {
            Debug.WriteLine($"Betöltés WorkoutPlan ID: {planId}");

            // WorkoutPlan betöltése
            var planResponse = await _supabase.From<WorkoutPlan>()
                .Where(x => x.Id == planId)
                .Single();
            if (planResponse == null)
            {
                Debug.WriteLine($"Hiba: Nincs WorkoutPlan a megadott ID-val: {planId}");
                throw new Exception($"Nincs edzésprogram a megadott azonosítóval: {planId}");
            }
            WorkoutPlan = planResponse;
            Debug.WriteLine($"WorkoutPlan betöltve: {WorkoutPlan.Title}, Days: {string.Join(", ", WorkoutPlan.Days ?? new List<string>())}");

            // Kapcsolódó Workouts betöltése
            var workoutResponse = await _supabase.From<Workout>()
                .Where(x => x.PlanId == planId)
                .Get();
            var workouts = workoutResponse.Models;
            Debug.WriteLine($"Workouts betöltve: {workouts.Count} db");

            // PlanDays összeállítása
            PlanDays.Clear();
            var days = WorkoutPlan.Days ?? new List<string>();
            if (!days.Any())
            {
                Debug.WriteLine("Figyelmeztetés: A WorkoutPlan Days lista üres vagy null");
                PlanDays.Add(new PlanDay
                {
                    Day = "N/A",
                    Workout = new Workout { Title = "Nincs edzés definiálva" }
                });
            }
            else
            {
                foreach (var day in days)
                {
                    var workoutForDay = workouts.FirstOrDefault(w => w.Day == day);
                    PlanDays.Add(new PlanDay
                    {
                        Day = day,
                        Workout = workoutForDay ?? new Workout { Title = "Nincs edzés erre a napra" }
                    });
                }
            }

            Debug.WriteLine($"WorkoutPlan sikeresen betöltve: {WorkoutPlan.Title}, {PlanDays.Count} nap");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"LoadPlanAsync hiba: {ex.Message}\nStackTrace: {ex.StackTrace}");
            throw; // A hibát továbbadjuk az OnAppearing-nek
        }
    }

    [RelayCommand]
    private async Task SelectPlanDay(PlanDay planDay)
    {
        if (planDay?.Workout == null || string.IsNullOrEmpty(planDay.Workout.Id))
        {
            Debug.WriteLine($"SelectPlanDay: Nincs érvényes Workout a naphoz: {planDay?.Day}");
            await Application.Current.MainPage.DisplayAlert("Hiba", "Ehhez a naphoz nincs edzés definiálva.", "OK");
            return;
        }

        try
        {
            Debug.WriteLine($"SelectPlanDay: Navigálás WorkoutDetailPage-re, WorkoutId: {planDay.Workout.Id}");
            var viewModel = new WorkoutDetailViewModel();
            await viewModel.LoadWorkoutAsync(planDay.Workout.Id);
            await Application.Current.MainPage.Navigation.PushAsync(new WorkoutDetailPage(viewModel));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"SelectPlanDay hiba: {ex.Message}\nStackTrace: {ex.StackTrace}");
            await Application.Current.MainPage.DisplayAlert("Hiba", $"Nem sikerült betölteni az edzést: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private void ToggleStart()
    {
        IsProgramStarted = !IsProgramStarted;
        Debug.WriteLine($"ToggleStart: Program állapota: {(IsProgramStarted ? "Started" : "Stopped")}");
    }



}




public class PlanDay
{
    public string Day { get; set; }
    public Workout Workout { get; set; }
}