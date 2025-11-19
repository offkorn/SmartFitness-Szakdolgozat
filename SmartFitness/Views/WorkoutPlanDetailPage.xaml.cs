using SmartFitness.ViewModels;
using Supabase;
using System;
using System.IO;

namespace SmartFitness.Views;

public partial class WorkoutPlanDetailPage : ContentPage
{
    private readonly WorkoutPlanDetailViewModel _viewModel;
    private readonly string _planId;

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
            await _viewModel.LoadPlanAsync(_planId);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hiba", $"Nem sikerült betölteni a programot: {ex.Message}", "OK");
    
        }
    }
}