using SmartFitness.ViewModels;
using SmartFitness.Models;
using Supabase;
using SmartFitness.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SmartFitness.Views;
using static SmartFitness.Pages.WorkoutSearchPage;

namespace SmartFitness.Pages;

public partial class WorkoutPage : ContentPage
{
    private readonly Client _supabase = SupabaseClient.Client;
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
            await LoadActiveProgram();  
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


    private async Task LoadActiveProgram()
    {
        if (App.CurrentUser == null)
        {
            ProgramFrame.IsVisible = false;
            NoProgramLabel.IsVisible = true;
            return;
        }

        var active = await ProgramService.GetActiveProgramAsync(App.CurrentUser.Id);

        if (active == null)
        {
            ProgramFrame.IsVisible = false;
            NoProgramLabel.IsVisible = true;
            return;
        }

        // Aktív program VAN  megjelenítjük
        ProgramFrame.IsVisible = true;
        NoProgramLabel.IsVisible = false;

        // Program adatainak betöltése
        var plan = await SupabaseClient.Client
            .From<WorkoutPlan>()
            .Where(x => x.Id == active.PlanId)
            .Single();

        ProgramTitleLabel.Text = plan.Title;
        ProgramStartedLabel.Text = $"started: {active.StartDate:yyyy.MM.dd}";

        
        ProgramFrame.BindingContext = plan.Id;
    }

    private async void OnProgramTapped(object sender, EventArgs e)
    {
        if (ProgramFrame.BindingContext is string planId)
        {
            await Navigation.PushAsync(
                new WorkoutPlanDetailPage(planId, SupabaseClient.Client)
            );
        }
    }


    private async void OnSearchBarTapped(object sender, EventArgs e)
    {
        var searchList = new List<SearchItem>();

        //  Edzések (Workouts)
        if (_viewModel.Workouts != null)
        {
            foreach (var w in _viewModel.Workouts)
            {
                searchList.Add(new SearchItem
                {
                    Title = w.Title,
                    ImageUrl = "workout_logo_icon.svg",

                    Subtitle = $"{w.DurationMinutes} min • {w.ExperienceLevel}",
                    OriginalData = w,
                    IsPlan = false
                });
            }
        }

        //  Programok (WorkoutPlans)
        if (_viewModel.WorkoutPlans != null)
        {
            foreach (var p in _viewModel.WorkoutPlans)
            {
                searchList.Add(new SearchItem
                {
                    Title = p.Title,
                    ImageUrl = "program_icon.svg",
                    // Itt pedig a heteket írjuk ki
                    Subtitle = $"{p.Weeks} Weeks • {p.ExperienceLevel}",
                    OriginalData = p,
                    IsPlan = true
                });
            }
        }

        var distinctItems = searchList.GroupBy(x => x.Title).Select(y => y.First()).ToList();
        await Navigation.PushAsync(new WorkoutSearchPage(distinctItems));
    }


    private async void OnSavedWorkoutsTapped(object sender, EventArgs e)
    {
        if (App.CurrentUser == null)
        {
            await DisplayAlert("Info", "Log in to see saved workouts", "OK");
            return;
        }

        try
        {

            var savedLinks = await _supabase.From<SavedWorkout>()
                .Where(x => x.UserId == App.CurrentUser.Id)
                .Get();

            var workoutIds = savedLinks.Models.Select(x => x.WorkoutId).ToList();

            if (!workoutIds.Any())
            {
                await DisplayAlert("Info", "You haven't saved any workouts yet.", "OK");
                return;
            }


            var workoutsResponse = await _supabase.From<Workout>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.In, workoutIds)
                .Get();

            var savedWorkoutsList = workoutsResponse.Models;

            await Navigation.PushAsync(new SavedWorkoutsPage(savedWorkoutsList.ToList()));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to load saved workouts: " + ex.Message, "OK");
        }
    }

}