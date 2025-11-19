using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartFitness.Models;
using System.Collections.ObjectModel;

namespace SmartFitness.ViewModels;

public partial class DayWorkoutViewModel : ObservableObject
{
    [ObservableProperty]
    private string day;

    [ObservableProperty]
    private ObservableCollection<Exercise> exercises;

    public DayWorkoutViewModel(string day, ObservableCollection<Exercise> exercises)
    {
        Day = day;
        Exercises = exercises ?? new ObservableCollection<Exercise>();
        if (Exercises.Count == 0)
        {
            AddExercise();
        }
    }

    [RelayCommand]
    private void AddExercise()
    {
        var exercise = new Exercise
        {
            Name = string.Empty,
            Sets = new ObservableCollection<ExerciseSet> { new ExerciseSet { SetNumber = 1 } },
            SubExercises = new ObservableCollection<Exercise>()
        };
        Exercises.Add(exercise);
    }

    [RelayCommand]
    private void AddSet(object parameter)
    {
        if (parameter is Exercise exercise)
        {
            var setNumber = exercise.Sets.Count + 1;
            exercise.Sets.Add(new ExerciseSet { SetNumber = setNumber });
        }
        else if (parameter is Exercise subExercise && subExercise.IsMainExercise == false)
        {
            var setNumber = subExercise.Sets.Count + 1;
            subExercise.Sets.Add(new ExerciseSet { SetNumber = setNumber });
        }
    }

    [RelayCommand]
    private void AddSuperset(Exercise exercise)
    {
        if (exercise != null)
        {
            var subExercise = new Exercise
            {
                Name = string.Empty,
                Sets = new ObservableCollection<ExerciseSet> { new ExerciseSet { SetNumber = 1 } },
                IsMainExercise = false
            };
            exercise.SubExercises.Add(subExercise);
        }
    }
}