using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartFitness.Models;
using System.Collections.ObjectModel;
using System.Linq;

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

    

    [RelayCommand]
    private void RemoveSet(Exercise exercise) 
    {
        if (exercise == null) return;
        if (exercise.Sets == null || exercise.Sets.Count == 0) return;

        exercise.Sets.RemoveAt(exercise.Sets.Count - 1);

        for (int i = 0; i < exercise.Sets.Count; i++)
        {
            exercise.Sets[i].SetNumber = i + 1;
        }
    }

    [RelayCommand]
    private void RemoveExercise(Exercise exercise) 
    {
        if (exercise == null) return;
        Exercises.Remove(exercise);
    }

    [RelayCommand]
    private void RemoveSubExercise(Exercise sub) 
    {
        if (sub == null) return;

        foreach (var ex in Exercises)
        {
            if (ex.SubExercises.Contains(sub))
            {
                ex.SubExercises.Remove(sub);
                return;
            }
        }
    }
}