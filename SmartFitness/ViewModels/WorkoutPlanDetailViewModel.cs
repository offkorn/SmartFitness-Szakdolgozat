using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartFitness.Models;
using SmartFitness.Pages;
using SmartFitness.Services;
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
        IsProgramStarted = false; // Alapértelmezetten a Start gomb van
       

    }

    public async Task LoadPlanAsync(string planId)
    {
        try
        {
            Debug.WriteLine($"Loading WorkoutPlan ID: {planId}");

            // WorkoutPlan betöltése
            var planResponse = await _supabase.From<WorkoutPlan>()
                .Where(x => x.Id == planId)
                .Single();
            if (planResponse == null)
            {
                Debug.WriteLine($"Error: no WorkoutPlan with ID: {planId}");
                throw new Exception($"No workoutplan with this id: {planId}");
            }
            WorkoutPlan = planResponse;
            Debug.WriteLine($"WorkoutPlan loadoed: {WorkoutPlan.Title}, Days: {string.Join(", ", WorkoutPlan.Days ?? new List<string>())}");

            // Kapcsolódó Workouts betöltése
            var workoutResponse = await _supabase.From<Workout>()
                .Where(x => x.PlanId == planId)
                .Get();
            var workouts = workoutResponse.Models;
            Debug.WriteLine($"Workouts loaded: {workouts.Count}");

            // PlanDays összeállítása
            PlanDays.Clear();
            var days = WorkoutPlan.Days ?? new List<string>();
            if (!days.Any())
            {
                Debug.WriteLine("Warning: WorkoutPlan Days list empty or null");
                PlanDays.Add(new PlanDay
                {
                    Day = "N/A",
                    Workout = new Workout { Title = "No workout defined" }
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
                        Workout = workoutForDay ?? new Workout { Title = "No workout for today" }
                    });
                }
            }

            if (App.CurrentUser != null)
                await CheckIfProgramActive(planId, App.CurrentUser.Id);


            Debug.WriteLine($"WorkoutPlan succesfully loaded: {WorkoutPlan.Title}, {PlanDays.Count} day");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"LoadPlanAsync error: {ex.Message}\nStackTrace: {ex.StackTrace}");
            throw; // A hibát továbbadjuk az OnAppearing-nek
        }
    }

    [RelayCommand]
    private async Task SelectPlanDay(PlanDay planDay)
    {
        if (planDay?.Workout == null || string.IsNullOrEmpty(planDay.Workout.Id))
        {
            Debug.WriteLine($"SelectPlanDay: No valid workout for this day: {planDay?.Day}");
            await Application.Current.MainPage.DisplayAlert("Error", "No workout for this day.", "OK");
            return;
        }

        try
        {
            Debug.WriteLine($"SelectPlanDay: Navigate to WorkoutDetailPage, WorkoutId: {planDay.Workout.Id}");
            var viewModel = new WorkoutDetailViewModel();
            await viewModel.LoadWorkoutAsync(planDay.Workout.Id);
            await Application.Current.MainPage.Navigation.PushAsync(new WorkoutDetailPage(viewModel));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"SelectPlanDay error: {ex.Message}\nStackTrace: {ex.StackTrace}");
            await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load workout: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private void ToggleStart()
    {
        IsProgramStarted = !IsProgramStarted;
        Debug.WriteLine($"ToggleStart: Program state: {(IsProgramStarted ? "Started" : "Stopped")}");
    }


    public async Task CheckIfProgramActive(string planId, string userId)
    {
        var active = await ProgramService.GetActiveProgramAsync(userId);

        if (active == null)
        {
            IsProgramStarted = false;
            return;
        }

        IsProgramStarted = (active.PlanId == planId);
    }

    
}

public class PlanDay
{
    public string Day { get; set; }
    public Workout Workout { get; set; }
}