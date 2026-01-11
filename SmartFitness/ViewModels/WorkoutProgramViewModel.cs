using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartFitness.Models;
using Supabase;
using SmartFitness.Services;
using SmartFitness.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SmartFitness.ViewModels;

public partial class WorkoutProgramViewModel : ObservableObject
{
    private readonly Client _supabase = SupabaseClient.Client;

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string location = "Gym";

    [ObservableProperty]
    private ObservableCollection<string> equipment = new();

    [ObservableProperty]
    private int durationMinutes;

    [ObservableProperty]
    private bool strength;
    [ObservableProperty]
    private bool muscleBuild;
    [ObservableProperty]
    private bool cardio;
    [ObservableProperty]
    private bool healthierLife;

    [ObservableProperty]
    private string experienceLevel = "Beginner";

    [ObservableProperty]
    private bool monday;
    [ObservableProperty]
    private bool tuesday;
    [ObservableProperty]
    private bool wednesday;
    [ObservableProperty]
    private bool thursday;
    [ObservableProperty]
    private bool friday;
    [ObservableProperty]
    private bool saturday;
    [ObservableProperty]
    private bool sunday;

    [ObservableProperty]
    private int weeks;

    // Ideiglenes tároló a napokhoz tartozó edzésekhez
    private Dictionary<string, ObservableCollection<Exercise>> _dayExercises = new();

    public WorkoutProgramViewModel()
    {
        Title = string.Empty;
    }

    public void SetLocation(string location)
    {
        Location = location;
    }

    public void UpdateEquipment(string equipment, bool isChecked)
    {
        if (isChecked && !Equipment.Contains(equipment))
        {
            Equipment.Add(equipment);
        }
        else if (!isChecked && Equipment.Contains(equipment))
        {
            Equipment.Remove(equipment);
        }
    }

    private List<string> GetSelectedDays()
    {
        var days = new List<string>();
        if (Monday)
        {
            days.Add("Monday");
            if (!_dayExercises.ContainsKey("Monday"))
                _dayExercises["Monday"] = new ObservableCollection<Exercise>();
        }
        if (Tuesday)
        {
            days.Add("Tuesday");
            if (!_dayExercises.ContainsKey("Tuesday"))
                _dayExercises["Tuesday"] = new ObservableCollection<Exercise>();
        }
        if (Wednesday)
        {
            days.Add("Wednesday");
            if (!_dayExercises.ContainsKey("Wednesday"))
                _dayExercises["Wednesday"] = new ObservableCollection<Exercise>();
        }
        if (Thursday)
        {
            days.Add("Thursday");
            if (!_dayExercises.ContainsKey("Thursday"))
                _dayExercises["Thursday"] = new ObservableCollection<Exercise>();
        }
        if (Friday)
        {
            days.Add("Friday");
            if (!_dayExercises.ContainsKey("Friday"))
                _dayExercises["Friday"] = new ObservableCollection<Exercise>();
        }
        if (Saturday)
        {
            days.Add("Saturday");
            if (!_dayExercises.ContainsKey("Saturday"))
                _dayExercises["Saturday"] = new ObservableCollection<Exercise>();
        }
        if (Sunday)
        {
            days.Add("Sunday");
            if (!_dayExercises.ContainsKey("Sunday"))
                _dayExercises["Sunday"] = new ObservableCollection<Exercise>();
        }
        return days;
    }

    private List<string> GetGoals()
    {
        var goals = new List<string>();
        if (Strength) goals.Add("Strength");
        if (MuscleBuild) goals.Add("Muscle Build");
        if (Cardio) goals.Add("Cardio");
        if (HealthierLife) goals.Add("Healthier Life");
        return goals;
    }




    [RelayCommand]
    public async Task SaveWorkoutPlan()
    {
        try
        {
            if (App.CurrentUser == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "You must be logged in to save plans!", "OK");
                return;
            }

            if (string.IsNullOrEmpty(Title))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Plan title is required!", "OK");
                return;
            }

            var plan = new WorkoutPlan
            {
                Id = Guid.NewGuid().ToString(),
                Title = Title,
                Location = Location,
                Equipment = Equipment?.ToList() ?? new List<string>(),
                DurationMinutes = DurationMinutes,
                Goals = GetGoals() ?? new List<string>(),
                ExperienceLevel = ExperienceLevel,
                Days = GetSelectedDays() ?? new List<string>(),
                Weeks = Weeks,
                CreatedBy = App.CurrentUser.Id,
                CreatedAt = DateTime.UtcNow
            };

            // Naplózzuk a plan.Id-t
            File.WriteAllText(Path.Combine(FileSystem.AppDataDirectory, "plan_id.log"),
                $"Plan ID: {plan.Id}");

            // Mentsük a WorkoutPlan-t és ellenőrizzük a beszúrást
            try
            {
                var response = await _supabase.From<WorkoutPlan>().Insert(plan, new Supabase.Postgrest.QueryOptions { Returning = Supabase.Postgrest.QueryOptions.ReturnType.Representation });
                if (response.Models == null || response.Models.Count == 0)
                {
                    throw new Exception("Failed to insert WorkoutPlan: No record returned.");
                }
                plan = response.Models[0]; 
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to save workout plan: {ex.Message}", "OK");

                return;
            }

            // Ellenőrizzük, hogy a plan.Id érvényes
            if (string.IsNullOrEmpty(plan.Id))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Invalid plan ID after insertion!", "OK");
                return;
            }

            // Ellenőrizzük, hogy van-e edzés a napokhoz
            foreach (var day in plan.Days)
            {
                if (!_dayExercises.ContainsKey(day))
                {
                    _dayExercises[day] = new ObservableCollection<Exercise>();
                }

                var exercises = _dayExercises[day];
                if (exercises == null)
                {
                    exercises = new ObservableCollection<Exercise>();
                    _dayExercises[day] = exercises;
                }

                if (exercises.Any() && exercises.Any(ex => string.IsNullOrEmpty(ex.Name)))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"All exercises for {day} must have a name!", "OK");
                    return;
                }

                var workout = new Workout
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = $"{day} Workout",
                    Location = Location,
                    Equipment = Equipment?.ToList() ?? new List<string>(),
                    DurationMinutes = DurationMinutes,
                    Goals = GetGoals() ?? new List<string>(),
                    ExperienceLevel = ExperienceLevel,
                    CreatedBy = App.CurrentUser.Id,
                    CreatedAt = DateTime.UtcNow,
                    Exercises = exercises.ToList(),
                    ImageUrl = null,
                    PlanId = plan.Id,
                    Day = day
                };


                // Mentsük a Workout-ot
                try
                {

                    var workoutResponse = await _supabase
                        .From<Workout>()
                        .Insert(workout, new Supabase.Postgrest.QueryOptions { Returning = Supabase.Postgrest.QueryOptions.ReturnType.Representation });

                    var savedWorkout = workoutResponse.Models.FirstOrDefault();

                    if (savedWorkout != null)
                    {
                        // kapcsolat mentése a plan_workouts táblába
                        var link = new PlanWorkout
                        {
                            Id = Guid.NewGuid().ToString(),

                            PlanId = plan.Id,
                            WorkoutId = savedWorkout.Id,
                            Day = day,
                            UserId = App.CurrentUser.Id
                        };

                        await _supabase.From<PlanWorkout>().Insert(link);
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to save workout linkage for {day}: {ex.Message}", "OK");
                    return;
                }
            }

            //await Application.Current.MainPage.DisplayAlert("Success", "Workout plan saved!", "OK");
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Failed to save plan: {ex.Message}", "OK");
        }
    }





    [RelayCommand]
    private async Task NavigateToDayWorkout(string day)
    {
        var exercises = _dayExercises.ContainsKey(day) ? _dayExercises[day] : new ObservableCollection<Exercise>();
        var page = new DayWorkoutPage(day, exercises, updatedExercises =>
        {
            _dayExercises[day] = updatedExercises;
        });
        await Application.Current.MainPage.Navigation.PushAsync(page);
    }

    [RelayCommand]
    private void IncreaseWeeks()
    {
        Weeks++;
    }

    [RelayCommand]
    private void DecreaseWeeks()
    {
        if (Weeks > 0)
            Weeks--;
    }
}