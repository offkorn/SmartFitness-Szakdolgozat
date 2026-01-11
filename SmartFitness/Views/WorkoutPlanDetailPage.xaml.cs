using SmartFitness.Services;
using SmartFitness.ViewModels;
using SmartFitness.Models;
using Supabase;
using System;
using System.Diagnostics;

namespace SmartFitness.Views;

public partial class WorkoutPlanDetailPage : ContentPage
{
    private readonly WorkoutPlanDetailViewModel _viewModel;
    private readonly string _planId;

    private WorkoutPlan _plan;   // itt tároljuk a tényleges tervet

    public WorkoutPlanDetailPage(string planId, Client supabase)
    {
        InitializeComponent();
        _viewModel = new WorkoutPlanDetailViewModel(supabase);
        BindingContext = _viewModel;
        _planId = planId;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            Debug.WriteLine($"[WorkoutPlanDetailPage] OnAppearing - loading planId = {_planId}");

            await _viewModel.LoadPlanAsync(_planId);

            _plan = _viewModel.WorkoutPlan;

            if (_plan == null)
            {
                Debug.WriteLine("[WorkoutPlanDetailPage] WorkoutPlan is NULL after loading the plan");
                await DisplayAlert("Error", "Failed to load workout plan.", "OK");
                return;
            }

            Debug.WriteLine($"[WorkoutPlanDetailPage] loaded plan: Id={_plan.Id}, Title={_plan.Title}, Weeks={_plan.Weeks}");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hiba", $"Nem sikerült betölteni a programot: {ex.Message}", "OK");
        }
    }

    private async void StartProgramButton_Clicked(object sender, EventArgs e)
    {
        var user = App.CurrentUser;
        if (user == null)
        {
            await DisplayAlert("Error", "User not logged in.", "OK");
            return;
        }

        if (_plan == null)
        {
            await DisplayAlert("Error", "Plan not loaded.", "OK");
            return;
        }

        // 1) Ha a program már aktív  Cancel funkció
        var active = await ProgramService.GetActiveProgramAsync(user.Id);

        if (active != null && active.PlanId == _plan.Id)
        {
            bool confirm = await DisplayAlert(
                "Cancel Program?",
                "Are you sure you want to cancel your current workout program?",
                "Yes", "No"
            );

            if (!confirm)
                return;

            await ProgramService.DeactivateProgramAsync(active.Id);

            await DisplayAlert("Canceled", "Your workout program has been canceled.", "OK");

            await _viewModel.CheckIfProgramActive(_plan.Id, user.Id);
            return;
        }

        // Ha még nincs aktív  Start Plan
        int weeks = _plan.Weeks ?? 3;

        // ha van, akar-e valtani vagy sem
        if (active != null)
        {
            bool confirm = await DisplayAlert(
                "Change Program?",
                "You already have an active program. Switch to this one?",
                "Yes", "No"
            );

            if (!confirm)
                return;
        }

        await ProgramService.StartProgramAsync(user.Id, _plan.Id, weeks);

        await DisplayAlert("Success", "Your new workout program has started!", "OK");

        await _viewModel.CheckIfProgramActive(_plan.Id, user.Id);
    }


}
