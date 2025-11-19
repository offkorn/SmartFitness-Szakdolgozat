using SmartFitness.Models;
using Supabase;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using SmartFitness.Views;

namespace SmartFitness.Pages;

public partial class AllWorkoutPlansPage : ContentPage
{
    private readonly Client _supabase;
    private ObservableCollection<WorkoutPlan> WorkoutPlans { get; } = new();
    public ICommand ViewWorkoutPlanCommand { get; }

    public AllWorkoutPlansPage(Client supabase)
    {
        InitializeComponent();
        _supabase = supabase;
        BindingContext = this;
        ViewWorkoutPlanCommand = new Command<WorkoutPlan>(async (plan) => await ViewWorkoutPlan(plan));
        LoadWorkoutPlans();
    }

    private async void LoadWorkoutPlans()
    {
        try
        {
            Debug.WriteLine("AllWorkoutPlansPage: WorkoutPlan-ek betöltése...");
            var planResponse = await _supabase.From<WorkoutPlan>()
                .Order(x => x.CreatedAt, Supabase.Postgrest.Constants.Ordering.Descending)
                .Range(0, 49)
                .Get();

            WorkoutPlans.Clear();
            foreach (var plan in planResponse.Models)
            {
                WorkoutPlans.Add(plan);
            }

            WorkoutPlansCollection.ItemsSource = WorkoutPlans;
            Debug.WriteLine($"AllWorkoutPlansPage: {WorkoutPlans.Count} WorkoutPlan betöltve");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"LoadWorkoutPlans hiba: {ex.Message}\nStackTrace: {ex.StackTrace}");
            await DisplayAlert("Hiba", $"Nem sikerült betölteni a programokat: {ex.Message}", "OK");
        }
    }

    private async Task ViewWorkoutPlan(WorkoutPlan plan)
    {
        if (plan != null)
        {
            await Navigation.PushAsync(new WorkoutPlanDetailPage(plan.Id, _supabase));
        }
    }
}