using System.Collections.ObjectModel;
using System.Windows.Input;
using SmartFitness.Models;
using SmartFitness.Services;
using Supabase;

namespace SmartFitness.Pages;

public partial class BreakfastPage : ContentPage
{
    private readonly Client _supabase = SupabaseClient.Client;
    private ObservableCollection<Meal> breakfastMeals = new();
    public ICommand ViewRecipeCommand { get; }

    public BreakfastPage()
    {
        InitializeComponent();
        BindingContext = this;
        ViewRecipeCommand = new Command<Meal>(async (meal) => await ViewRecipe(meal));
        LoadBreakfastMeals();
    }

    private async void LoadBreakfastMeals()
    {
        try
        {
            var mealResponse = await _supabase.From<Meal>()
                .Where(m => m.Type == "BREAKFAST") // Match exact case as stored in DB
                .Get();

            breakfastMeals.Clear();
            foreach (var meal in mealResponse.Models)
            {
                if (string.IsNullOrEmpty(meal.AuthorName))
                    meal.AuthorName = "Unknown";
                breakfastMeals.Add(meal);
            }

            BreakfastRecipes.ItemsSource = breakfastMeals;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load breakfast meals: {ex.Message}", "OK");
        }
    }

    private async Task ViewRecipe(Meal meal)
    {
        await Navigation.PushAsync(new RecipeDetailPage(meal));
    }
}