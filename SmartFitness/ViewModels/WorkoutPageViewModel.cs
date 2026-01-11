using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartFitness.Models;
using System.Collections.ObjectModel;
using Supabase;
using SmartFitness.Services;
using SmartFitness.Pages;
using System.Diagnostics;
using System.Linq;
using SmartFitness.Views;

namespace SmartFitness.ViewModels;

public partial class WorkoutPageViewModel : ObservableObject
{
    private readonly Client _supabase;

    [ObservableProperty]
    private ObservableCollection<Workout> workouts = new();

    [ObservableProperty]
    private ObservableCollection<WorkoutPlan> workoutPlans = new();

    [ObservableProperty]
    private ObservableCollection<Workout> recommendedWorkouts = new();

    [ObservableProperty]
    private ObservableCollection<WorkoutPlan> recommendedWorkoutPlans = new();

    public WorkoutPageViewModel()
    {
        _supabase = SupabaseClient.Client;
    }

    public WorkoutPageViewModel(Client supabase)
    {
        _supabase = supabase;
    }

    public async Task LoadWorkoutsAndPlansAsync(bool loadWorkouts = true)
    {
        try
        {
            // Felhasználói azonosító ellenőrzése
            if (!Guid.TryParse(App.CurrentUser.Id, out _))
            {
                Debug.WriteLine($"Error: Érvénytelen UUID formátum: {App.CurrentUser.Id}");
                await Application.Current.MainPage.DisplayAlert("Error", "Érvénytelen felhasználói azonosító!", "OK");
                return;
            }

            // SimpleWorkout-ok betöltése (csak ha szükséges)
            if (loadWorkouts)
            {
                Debug.WriteLine("Edzések betöltése...");
                var workoutResponse = await _supabase.From<Workout>().Get();
                Workouts.Clear();
                foreach (var workout in workoutResponse.Models)
                {
                    Workouts.Add(workout);
                }
                Debug.WriteLine($"Edzések sikeresen betöltve: {Workouts.Count} db");
                if (Workouts.Count == 0)
                {
                    Debug.WriteLine("[DEBUG] Warning: Egyetlen edzés sem töltődött be az adatbázisból!");
                }
            }

            // Összes WorkoutPlan betöltése 
            Debug.WriteLine("WorkoutPlan-ek betöltése...");
            var planResponse = await _supabase.From<WorkoutPlan>()
            .Order(x => x.CreatedAt, Supabase.Postgrest.Constants.Ordering.Descending)
            .Range(0, 49)
            .Get();
            WorkoutPlans.Clear();
            foreach (var plan in planResponse.Models)
            {
                WorkoutPlans.Add(plan);
            }
            Debug.WriteLine($"Programok sikeresen betöltve: {WorkoutPlans.Count} db");
            if (WorkoutPlans.Count == 0)
            {
                Debug.WriteLine("[DEBUG] Warning: Egyetlen program sem töltődött be az adatbázisból!");
            }

            // Felhasználói preferenciák lekérése és szűrés
            Debug.WriteLine("Felhasználói edzés preferenciák betöltése...");
            var preferences = await _supabase.From<WorkoutModel>()
            .Where(p => p.UserId == App.CurrentUser.Id)
            .Single();

            if (preferences == null)
            {
                Debug.WriteLine("[DEBUG] Nem található edzés preferencia a felhasználóhoz");
                await Application.Current.MainPage.DisplayAlert("Info", "Nincsenek edzés preferenciák megadva. Kérlek, állítsd be a preferenciáidat.", "OK");
                return;
            }

            // Ajánlott edzések és programok szűrése
            await FilterRecommendedWorkouts(preferences);
            await FilterRecommendedWorkoutPlans(preferences);

            if (RecommendedWorkouts.Count == 0)
            {
                Debug.WriteLine("[DEBUG] Nincs ajánlott edzés, mert egyik edzés sem felelt meg a szűrési feltételeknek.");
            }
            if (RecommendedWorkoutPlans.Count == 0)
            {
                Debug.WriteLine("[DEBUG] Nincs ajánlott program, mert egyik program sem felelt meg a szűrési feltételeknek.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"LoadWorkoutsAndPlansAsync hiba: {ex.Message}\nStackTrace: {ex.StackTrace}");
            await Application.Current.MainPage.DisplayAlert("Hiba", $"Nem sikerült betölteni az adatokat: {ex.Message}", "OK");
        }
    }

    private async Task FilterRecommendedWorkouts(WorkoutModel preferences)
    {
        try
        {
            Debug.WriteLine("[DEBUG] Ajánlott edzések szűrésének kezdete...");
            RecommendedWorkouts.Clear();

            // Edzéstípus és felszerelés megfeleltetése
            var equipmentMapping = new Dictionary<string, List<string>>
{
{ "bodyweight", new List<string> { "none", "bodyweight" } },
{ "machine", new List<string> { "machine", "cardio machine", "strength machine" } },
{ "weights", new List<string> { "dumbbell", "barbell", "kettlebell", "weight plate", "weights" } }
};

            // Preferenciák feldolgozása
            var workoutLocation = preferences.WorkoutLocation?.ToLower()?.Trim();
            var workoutType = preferences.WorkoutType?.ToLower()?.Trim();
            var workoutDuration = preferences.WorkoutDuration;
            var workoutExperience = preferences.WorkoutExperience?.ToLower()?.Trim();

            // WorkoutGoal feldolgozása: vesszővel elválasztott stringből lista
            var workoutGoals = preferences.WorkoutGoal?.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(g => g.ToLower().Trim())
            .ToList() ?? new List<string>();

            Debug.WriteLine($"[DEBUG] Feldolgozott preferenciák (edzések): WorkoutLocation = {workoutLocation ?? "null"}, WorkoutType = {workoutType ?? "null"}, WorkoutDuration = {workoutDuration}, WorkoutGoals = {(workoutGoals.Any() ? string.Join(", ", workoutGoals) : "null")}, WorkoutExperience = {workoutExperience ?? "null"}");

            // Edzések szűrése
            foreach (var workout in Workouts)
            {
                bool isSuitable = true;

                Debug.WriteLine($"[DEBUG] Edzés értékelése: {workout.Title}, ID = {workout.Id}, Location = {workout.Location ?? "null"}, Equipment = {(workout.Equipment != null ? string.Join(", ", workout.Equipment) : "null")}, Duration = {workout.DurationMinutes}, Goals = {(workout.Goals != null ? string.Join(", ", workout.Goals) : "null")}, ExperienceLevel = {workout.ExperienceLevel ?? "null"}");

                // Helyszín ellenőrzése
                if (isSuitable && !string.IsNullOrEmpty(workoutLocation))
                {
                    var workoutLocationNormalized = workout.Location?.ToLower()?.Trim();
                    if (!string.Equals(workoutLocationNormalized, workoutLocation, StringComparison.OrdinalIgnoreCase))
                    {
                        isSuitable = false;
                        Debug.WriteLine($"[DEBUG] Edzés {workout.Title} kizárva helyszín miatt: {workoutLocationNormalized} != {workoutLocation}");
                    }
                    else
                    {
                        Debug.WriteLine($"[DEBUG] Edzés {workout.Title} megfelel a helyszín szűrésnek: {workoutLocation}");
                    }
                }

                // Edzéstípus (felszerelés) ellenőrzése
                if (isSuitable && !string.IsNullOrEmpty(workoutType) && equipmentMapping.ContainsKey(workoutType))
                {
                    var requiredEquipment = equipmentMapping[workoutType];
                    if (workout.Equipment == null || !workout.Equipment.Any(e => requiredEquipment.Contains(e?.ToLower()?.Trim() ?? "") || e?.ToLower()?.Trim() == workoutType))
                    {
                        isSuitable = false;
                        Debug.WriteLine($"[DEBUG] Edzés {workout.Title} kizárva felszerelés miatt: Nem tartalmazza a szükséges felszerelést ({string.Join(", ", requiredEquipment)} vagy {workoutType})");
                    }
                    else
                    {
                        Debug.WriteLine($"[DEBUG] Edzés {workout.Title} megfelel a felszerelés szűrésnek: {workoutType}");
                    }
                }

                // Időtartam ellenőrzése
                if (isSuitable && workoutDuration > 0)
                {
                    if (workout.DurationMinutes > workoutDuration)
                    {
                        isSuitable = false;
                        Debug.WriteLine($"[DEBUG] Edzés {workout.Title} kizárva időtartam miatt: {workout.DurationMinutes} > {workoutDuration}");
                    }
                    else
                    {
                        Debug.WriteLine($"[DEBUG] Edzés {workout.Title} megfelel az időtartam szűrésnek: {workout.DurationMinutes} <= {workoutDuration}");
                    }
                }

                // Cél ellenőrzése
                if (isSuitable && workoutGoals.Any())
                {
                    var workoutGoalsNormalized = workout.Goals?.Select(g => g?.ToLower()?.Trim()).ToList();
                    if (workoutGoalsNormalized == null || !workoutGoalsNormalized.Any(wg => workoutGoals.Contains(wg)))
                    {
                        isSuitable = false;
                        Debug.WriteLine($"[DEBUG] Edzés {workout.Title} kizárva cél miatt: Nem tartalmazza a szükséges célokat ({string.Join(", ", workoutGoals)})");
                    }
                    else
                    {
                        Debug.WriteLine($"[DEBUG] Edzés {workout.Title} megfelel a cél szűrésnek: Egyező célok = {string.Join(", ", workoutGoalsNormalized.Intersect(workoutGoals))}");
                    }
                }

                // Tapasztalati szint ellenőrzése
                if (isSuitable && !string.IsNullOrEmpty(workoutExperience))
                {
                    var workoutExperienceNormalized = workout.ExperienceLevel?.ToLower()?.Trim();
                    if (!string.Equals(workoutExperienceNormalized, workoutExperience, StringComparison.OrdinalIgnoreCase))
                    {
                        isSuitable = false;
                        Debug.WriteLine($"[DEBUG] Edzés {workout.Title} kizárva tapasztalati szint miatt: {workoutExperienceNormalized} != {workoutExperience}");
                    }
                    else
                    {
                        Debug.WriteLine($"[DEBUG] Edzés {workout.Title} megfelel a tapasztalati szint szűrésnek: {workoutExperience}");
                    }
                }

                // Gyűjteményhez adás
                if (isSuitable)
                {
                    RecommendedWorkouts.Add(workout);
                    Debug.WriteLine($"[DEBUG] Edzés {workout.Title} hozzáadva az ajánlott edzésekhez");
                }
                else
                {
                    Debug.WriteLine($"[DEBUG] Edzés {workout.Title} nem került hozzáadásra az ajánlott edzésekhez");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"FilterRecommendedWorkouts hiba: {ex.Message}\nStackTrace: {ex.StackTrace}");
            await Application.Current.MainPage.DisplayAlert("Hiba", $"Nem sikerült szűrni az edzéseket: {ex.Message}", "OK");
        }
    }

    private async Task FilterRecommendedWorkoutPlans(WorkoutModel preferences)
    {
        try
        {
            Debug.WriteLine("[DEBUG] Ajánlott programok szűrésének kezdete...");
            RecommendedWorkoutPlans.Clear();

            // Edzéstípus és felszerelés megfeleltetése
            var equipmentMapping = new Dictionary<string, List<string>>
            {
            { "bodyweight", new List<string> { "none", "bodyweight" } },
            { "machine", new List<string> { "machine", "cardio machine", "strength machine" } },
            { "weights", new List<string> { "dumbbell", "barbell", "kettlebell", "weight plate", "weights" } }
            };

            // Preferenciák feldolgozása
            var workoutLocation = preferences.WorkoutLocation?.ToLower()?.Trim();
            var workoutType = preferences.WorkoutType?.ToLower()?.Trim();
            var workoutDuration = preferences.WorkoutDuration;
            var workoutExperience = preferences.WorkoutExperience?.ToLower()?.Trim();

            // WorkoutGoal feldolgozása: vesszővel elválasztott stringből lista
            var workoutGoals = preferences.WorkoutGoal?.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(g => g.ToLower().Trim())
            .ToList() ?? new List<string>();

            // WorkoutDays feldolgozása: stringből int
            int workoutDaysCount = 0;
            if (!string.IsNullOrEmpty(preferences.WorkoutDays) && int.TryParse(preferences.WorkoutDays, out var parsedDays))
            {
                workoutDaysCount = parsedDays;
            }

            Debug.WriteLine($"[DEBUG] Feldolgozott preferenciák (programok): WorkoutLocation = {workoutLocation ?? "null"}, WorkoutType = {workoutType ?? "null"}, WorkoutDuration = {workoutDuration}, WorkoutGoals = {(workoutGoals.Any() ? string.Join(", ", workoutGoals) : "null")}, WorkoutExperience = {workoutExperience ?? "null"}, WorkoutDays = {workoutDaysCount}");

            // Programok szűrése
            foreach (var plan in WorkoutPlans)
            {
                bool isSuitable = true;

                Debug.WriteLine($"[DEBUG] Program értékelése: {plan.Title}, ID = {plan.Id}, Location = {plan.Location ?? "null"}, Equipment = {(plan.Equipment != null ? string.Join(", ", plan.Equipment) : "null")}, Duration = {plan.DurationMinutes ?? 0}, Goals = {(plan.Goals != null ? string.Join(", ", plan.Goals) : "null")}, ExperienceLevel = {plan.ExperienceLevel ?? "null"}, Days = {(plan.Days != null ? string.Join(", ", plan.Days) : "null")} ({plan.Days?.Count ?? 0} nap), Weeks = {plan.Weeks ?? 0}");

                // Helyszín ellenőrzése
                if (isSuitable && !string.IsNullOrEmpty(workoutLocation))
                {
                    var planLocationNormalized = plan.Location?.ToLower()?.Trim();
                    if (!string.Equals(planLocationNormalized, workoutLocation, StringComparison.OrdinalIgnoreCase))
                    {
                        isSuitable = false;
                        Debug.WriteLine($"[DEBUG] Program {plan.Title} kizárva helyszín miatt: {planLocationNormalized} != {workoutLocation}");
                    }
                    else
                    {
                        Debug.WriteLine($"[DEBUG] Program {plan.Title} megfelel a helyszín szűrésnek: {workoutLocation}");
                    }
                }

                // Edzéstípus (felszerelés) ellenőrzése
                if (isSuitable && !string.IsNullOrEmpty(workoutType) && equipmentMapping.ContainsKey(workoutType))
                {
                    var requiredEquipment = equipmentMapping[workoutType];
                    if (plan.Equipment == null || !plan.Equipment.Any(e => requiredEquipment.Contains(e?.ToLower()?.Trim() ?? "") || e?.ToLower()?.Trim() == workoutType))
                    {
                        isSuitable = false;
                        Debug.WriteLine($"[DEBUG] Program {plan.Title} kizárva felszerelés miatt: Nem tartalmazza a szükséges felszerelést ({string.Join(", ", requiredEquipment)} vagy {workoutType})");
                    }
                    else
                    {
                        Debug.WriteLine($"[DEBUG] Program {plan.Title} megfelel a felszerelés szűrésnek: {workoutType}");
                    }
                }

                // Időtartam ellenőrzése
                if (isSuitable && workoutDuration > 0 && plan.DurationMinutes.HasValue)
                {
                    if (plan.DurationMinutes > workoutDuration)
                    {
                        isSuitable = false;
                        Debug.WriteLine($"[DEBUG] Program {plan.Title} kizárva időtartam miatt: {plan.DurationMinutes} > {workoutDuration}");
                    }
                    else
                    {
                        Debug.WriteLine($"[DEBUG] Program {plan.Title} megfelel az időtartam szűrésnek: {plan.DurationMinutes} <= {workoutDuration}");
                    }
                }

                // Cél ellenőrzése
                if (isSuitable && workoutGoals.Any())
                {
                    var planGoalsNormalized = plan.Goals?.Select(g => g?.ToLower()?.Trim()).ToList();
                    if (planGoalsNormalized == null || !planGoalsNormalized.Any(pg => workoutGoals.Contains(pg)))
                    {
                        isSuitable = false;
                        Debug.WriteLine($"[DEBUG] Program {plan.Title} kizárva cél miatt: Nem tartalmazza a szükséges célokat ({string.Join(", ", workoutGoals)})");
                    }
                    else
                    {
                        Debug.WriteLine($"[DEBUG] Program {plan.Title} megfelel a cél szűrésnek: Egyező célok = {string.Join(", ", planGoalsNormalized.Intersect(workoutGoals))}");
                    }
                }

                // Tapasztalati szint ellenőrzése
                if (isSuitable && !string.IsNullOrEmpty(workoutExperience))
                {
                    var planExperienceNormalized = plan.ExperienceLevel?.ToLower()?.Trim();
                    if (!string.Equals(planExperienceNormalized, workoutExperience, StringComparison.OrdinalIgnoreCase))
                    {
                        isSuitable = false;
                        Debug.WriteLine($"[DEBUG] Program {plan.Title} kizárva tapasztalati szint miatt: {planExperienceNormalized} != {workoutExperience}");
                    }
                    else
                    {
                        Debug.WriteLine($"[DEBUG] Program {plan.Title} megfelel a tapasztalati szint szűrésnek: {workoutExperience}");
                    }
                }

                // Napok száma ellenőrzése (pontos egyezés)
                if (isSuitable && workoutDaysCount > 0)
                {
                    var planDaysCount = plan.Days?.Count ?? 0;
                    if (planDaysCount != workoutDaysCount)
                    {
                        isSuitable = false;
                        Debug.WriteLine($"[DEBUG] Program {plan.Title} kizárva napok száma miatt: {planDaysCount} != {workoutDaysCount}");
                    }
                    else
                    {
                        Debug.WriteLine($"[DEBUG] Program {plan.Title} megfelel a napok száma szűrésnek: {planDaysCount} == {workoutDaysCount}");
                    }
                }

                // Gyűjteményhez adás
                if (isSuitable)
                {
                    RecommendedWorkoutPlans.Add(plan);
                    Debug.WriteLine($"[DEBUG] Program {plan.Title} hozzáadva az ajánlott programokhoz");
                }
                else
                {
                    Debug.WriteLine($"[DEBUG] Program {plan.Title} nem került hozzáadásra az ajánlott programokhoz");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"FilterRecommendedWorkoutPlans hiba: {ex.Message}\nStackTrace: {ex.StackTrace}");
            await Application.Current.MainPage.DisplayAlert("Hiba", $"Nem sikerült szűrni a programokat: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task AddWorkout()
    {
        await Application.Current.MainPage.Navigation.PushAsync(new MakeWorkoutPage());
    }

    [RelayCommand]
    private async Task WorkoutSelected(Workout workout)
    {
        if (workout != null)
        {
            var viewModel = new WorkoutDetailViewModel();
            await viewModel.LoadWorkoutAsync(workout.Id);
            await Application.Current.MainPage.Navigation.PushAsync(new WorkoutDetailPage(viewModel));
        }
    }

    [RelayCommand]
    private async Task WorkoutPlanSelected(WorkoutPlan plan)
    {
        if (plan != null)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new WorkoutPlanDetailPage(plan.Id, _supabase));
        }
    }

    [RelayCommand]
    private async Task ViewAllWorkouts()
    {
        await Application.Current.MainPage.Navigation.PushAsync(new AllWorkoutsPage(_supabase));
    }

    [RelayCommand]
    private async Task ViewAllPrograms()
    {
        await Application.Current.MainPage.Navigation.PushAsync(new AllWorkoutPlansPage(_supabase));
    }
}