using Microsoft.Maui.Controls;
using SmartFitness.Models;
using SmartFitness.ViewModels;
using System;
using System.Collections.ObjectModel;

namespace SmartFitness.Views;

public partial class DayWorkoutPage : ContentPage
{
    private readonly Action<ObservableCollection<Exercise>> _onExercisesUpdated;

    public DayWorkoutPage(string day, ObservableCollection<Exercise> exercises, Action<ObservableCollection<Exercise>> onExercisesUpdated)
    {
        InitializeComponent();
        BindingContext = new DayWorkoutViewModel(day, exercises);
        _onExercisesUpdated = onExercisesUpdated;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        var viewModel = BindingContext as DayWorkoutViewModel;
        _onExercisesUpdated?.Invoke(viewModel.Exercises);
    }
}