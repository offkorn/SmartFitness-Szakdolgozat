using SmartFitness.ViewModels;
using SmartFitness.Models;
using Supabase;
using SmartFitness.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartFitness.Pages;

public partial class WorkoutPage : ContentPage
{
    private readonly WorkoutPageViewModel _viewModel;

    public WorkoutPage()
        : this(new WorkoutPageViewModel())
    {
    }

    public WorkoutPage(WorkoutPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            await _viewModel.LoadWorkoutsAndPlansAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hiba", $"Nem sikerült betölteni az adatokat: {ex.Message}", "OK");
        }
    }

    private async void OnAddWorkoutTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MakeWorkoutPage());
    }
}