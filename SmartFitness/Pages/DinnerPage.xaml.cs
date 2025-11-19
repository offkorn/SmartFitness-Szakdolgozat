using System.Collections.ObjectModel;
using System.Windows.Input;
using SmartFitness.Models;
using SmartFitness.Services;
using Supabase;

namespace SmartFitness.Pages;

public partial class DinnerPage : ContentPage
{
    private readonly Client _supabase = SupabaseClient.Client;
    private ObservableCollection<Meal> dinnerMeals = new();
    public ICommand ViewRecipeCommand { get; }

    public DinnerPage()
    {
        InitializeComponent(); // XAML és code-behind összekapcsolása
        BindingContext = this;
        ViewRecipeCommand = new Command<Meal>(async (meal) => await ViewRecipe(meal));
        LoadDinnerMeals();
    }

    private async void LoadDinnerMeals()
    {
        try
        {
            // Egyszerû egyezés, feltételezve, hogy a Type "DINNER" formátumban van az adatbázisban
            var mealResponse = await _supabase.From<Meal>()
                .Where(m => m.Type == "DINNER")
                .Get();

            dinnerMeals.Clear();
            foreach (var meal in mealResponse.Models)
            {
                if (string.IsNullOrEmpty(meal.AuthorName))
                    meal.AuthorName = "Unknown";
                dinnerMeals.Add(meal);
            }

            DinnerRecipes.ItemsSource = dinnerMeals; // A CollectionView feltöltése
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load dinner meals: {ex.Message}", "OK");
            Console.WriteLine(ex.ToString()); // Részletes hiba a debug kimenetben
        }
    }

    private async Task ViewRecipe(Meal meal)
    {
        await Navigation.PushAsync(new RecipeDetailPage(meal));
    }
}