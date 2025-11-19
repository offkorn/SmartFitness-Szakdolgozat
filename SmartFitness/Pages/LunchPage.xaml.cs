using System.Collections.ObjectModel;
using System.Windows.Input;
using SmartFitness.Models;
using SmartFitness.Services;
using Supabase;

namespace SmartFitness.Pages;

public partial class LunchPage : ContentPage
{
    private readonly Client _supabase = SupabaseClient.Client;
    private ObservableCollection<Meal> lunchMeals = new();
    public ICommand ViewRecipeCommand { get; }

    public LunchPage()
    {
        InitializeComponent(); 
        BindingContext = this;
        ViewRecipeCommand = new Command<Meal>(async (meal) => await ViewRecipe(meal));
        LoadLunchMeals();
    }

    private async void LoadLunchMeals()
    {
        try
        {
            // Option 1: Egyszerû egyezés, feltételezve, hogy a Type "LUNCH" formátumban van az adatbázisban
            var mealResponse = await _supabase.From<Meal>()
                .Where(m => m.Type == "LUNCH")
                .Get();

            lunchMeals.Clear();
            foreach (var meal in mealResponse.Models)
            {
                if (string.IsNullOrEmpty(meal.AuthorName))
                    meal.AuthorName = "Unknown";
                lunchMeals.Add(meal);
            }

            LunchRecipes.ItemsSource = lunchMeals; // A CollectionView feltöltése
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load lunch meals: {ex.Message}", "OK");
            Console.WriteLine(ex.ToString()); // Részletes hiba a debug kimenetben
        }
    }

    private async Task ViewRecipe(Meal meal)
    {
        await Navigation.PushAsync(new RecipeDetailPage(meal));
    }
}