using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartFitness.Models;
using System;
using System.Collections.ObjectModel;
using Supabase;
using SmartFitness.Services;


namespace SmartFitness.ViewModels;

public partial class SimpleWorkoutViewModel : ObservableObject
{
    private readonly Client _supabase = SupabaseClient.Client;

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string location;

    [ObservableProperty]
    private ObservableCollection<string> equipment = new();

    [ObservableProperty]
    private int durationMinutes;

    [ObservableProperty]
    private ObservableCollection<string> goals = new();

    [ObservableProperty]
    private string experienceLevel;

    [ObservableProperty]
    private ObservableCollection<Exercise> exercises = new();

    public SimpleWorkoutViewModel()
    {
        Title = string.Empty;
        Location = "Gym";
        ExperienceLevel = "Beginner";
        Console.WriteLine("SimpleWorkoutViewModel initialized");
    }

    [RelayCommand]
    private void AddExercise()
    {
        try
        {
            Console.WriteLine("AddExerciseCommand triggered");
            var exercise = new Exercise
            {
                Id = Guid.NewGuid().ToString(),
                Name = ""
            };
            exercise.Sets.Add(new ExerciseSet
            {
                Id = Guid.NewGuid().ToString(),
                SetNumber = 1,
                Weight = 0,
                Repetitions = 0
            });
            Exercises.Add(exercise);
            Console.WriteLine($"Exercise added: {exercise.Name}, Total exercises: {Exercises.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AddExercise error: {ex.Message}");
        }
    }

    [RelayCommand]
    private void AddSet(Exercise exercise)
    {
        try
        {
            if (exercise == null)
            {
                Console.WriteLine("AddSetCommand: No exercise provided");
                return;
            }

            Console.WriteLine($"AddSetCommand triggered for exercise: {exercise.Name}");
            var lastSetNumber = exercise.Sets.Any() ? exercise.Sets.Max(s => s.SetNumber) : 0;
            var newSet = new ExerciseSet
            {
                Id = Guid.NewGuid().ToString(),
                SetNumber = lastSetNumber + 1,
                Weight = 0,
                Repetitions = 0
            };
            exercise.Sets.Add(newSet);
            Console.WriteLine($"Set added to {exercise.Name}, Set number: {newSet.SetNumber}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AddSet error: {ex.Message}");
        }
    }

    [RelayCommand]
    private void AddSuperset(Exercise exercise)
    {
        try
        {
            if (exercise == null)
            {
                Console.WriteLine("AddSupersetCommand: No exercise provided");
                return;
            }

            Console.WriteLine($"AddSupersetCommand triggered for exercise: {exercise.Name}");
            var subExercise = new Exercise
            {
                Id = Guid.NewGuid().ToString(),
                Name = ""
            };
            subExercise.Sets.Add(new ExerciseSet
            {
                Id = Guid.NewGuid().ToString(),
                SetNumber = 1,
                Weight = 0,
                Repetitions = 0
            });
            exercise.SubExercises.Add(subExercise);
            Console.WriteLine($"Sub-exercise added to {exercise.Name}: {subExercise.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AddSuperset error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task SaveWorkout()
    {
        try
        {
            Console.WriteLine("SaveWorkoutCommand triggered");
            if (App.CurrentUser == null)
            {
                Console.WriteLine("No user logged in");
                await Application.Current.MainPage.DisplayAlert("Error", "You must be logged in to save workouts!", "OK");
                return;
            }

            if (string.IsNullOrEmpty(Title))
            {
                Console.WriteLine("Workout title is empty");
                await Application.Current.MainPage.DisplayAlert("Error", "Workout title is required!", "OK");
                return;
            }

            var workout = new Workout
            {
                Id = Guid.NewGuid().ToString(),
                Title = Title,
                ImageUrl = null,
                Location = Location,
                Equipment = Equipment.ToList(),
                DurationMinutes = DurationMinutes,
                Goals = Goals.ToList(),
                ExperienceLevel = ExperienceLevel,
                CreatedBy = App.CurrentUser.Id,
                CreatedAt = DateTime.UtcNow,
                Exercises = Exercises.ToList(),
                PlanId = null, // SimpleWorkout esetén null
                Day = null     // SimpleWorkout esetén null
            };

            
            await _supabase.From<Workout>().Insert(workout);

            await Application.Current.MainPage.DisplayAlert("Success", "Workout saved!", "OK");
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Failed to save workout: {ex.Message}", "OK");
        }
    }
}