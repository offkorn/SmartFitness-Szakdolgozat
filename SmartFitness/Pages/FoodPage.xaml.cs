using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SmartFitness.Models;
using SmartFitness.Services;
using Supabase;

namespace SmartFitness.Pages;

public partial class FoodPage : ContentPage
{
    private readonly Client _supabase = SupabaseClient.Client;
    private ObservableCollection<Meal> breakfastMeals = new();
    private ObservableCollection<Meal> lunchMeals = new();
    private ObservableCollection<Meal> dinnerMeals = new();
    private ObservableCollection<Meal> recommendedMeals = new();
    public ICommand ViewRecipeCommand { get; }

    public FoodPage()
    {
        InitializeComponent();
        BindingContext = this;

        ViewRecipeCommand = new Command<Meal>(async (meal) => await ViewRecipe(meal));

        AddLabelNavigation(AllBreakfastLabel, typeof(BreakfastPage));
        AddLabelNavigation(AllLunchLabel, typeof(LunchPage));
        AddLabelNavigation(AllDinnerLabel, typeof(DinnerPage));
    }

    private void AddLabelNavigation(Label label, Type pageType)
    {
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (s, e) =>
        {
            await Navigation.PushAsync((Page)Activator.CreateInstance(pageType));
        };
        label.GestureRecognizers.Add(tapGesture);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadMeals();
    }

    private async Task LoadMeals()
    {
        try
        {
            // Hibakeresési naplózás a munkamenet állapotáról
            Console.WriteLine($"[DEBUG] Munkamenet ellenõrzése: CurrentSession = {_supabase.Auth.CurrentSession != null}, User = {_supabase.Auth.CurrentSession?.User != null}");
            Console.WriteLine($"[DEBUG] App.CurrentUser ellenõrzése: User = {App.CurrentUser != null}, UserId = {App.CurrentUser?.Id}");

            // Ellenõrizzük, hogy App.CurrentUser létezik-e
            if (App.CurrentUser == null || string.IsNullOrEmpty(App.CurrentUser.Id))
            {
                Console.WriteLine("[DEBUG] App.CurrentUser null vagy nincs ID. Munkamenet frissítése folyamatban.");

                // Tartalék: Munkamenet lekérése vagy frissítése
                var session = _supabase.Auth.CurrentSession;
                if (session == null || session.User == null)
                {
                    try
                    {
                        session = await _supabase.Auth.RetrieveSessionAsync();
                        Console.WriteLine($"[DEBUG] Munkamenet frissítése megkísérelve: Success = {session != null}, User = {session?.User != null}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[DEBUG] Munkamenet frissítése sikertelen: {ex.Message}");
                    }

                    if (session == null || session.User == null)
                    {
                        await DisplayAlert("Hiba", "Felhasználó nincs bejelentkezve. Átirányítás a bejelentkezésre.", "OK");
                        return;
                    }
                }

                // App.CurrentUser frissítése, ha a munkamenet helyreállt
                App.CurrentUser = new User { Id = session.User.Id }; // Igazítsd a User modellhez
                Console.WriteLine($"[DEBUG] App.CurrentUser frissítve a munkamenetbõl: UserId = {App.CurrentUser.Id}");
            }

            var userId = App.CurrentUser.Id;
            Console.WriteLine($"[DEBUG] Felhasználó ID használata: {userId}");

            // Összes recept lekérése
            var mealResponse = await _supabase.From<Meal>().Get();
            var meals = mealResponse.Models;
            Console.WriteLine($"[DEBUG] {meals.Count} recept lekérve");

            // Felhasználói preferenciák lekérése
            var preferences = await _supabase.From<DietPreferences>()
                .Where(p => p.UserId == userId)
                .Single();

            if (preferences == null)
            {
                await DisplayAlert("Info", "Nincsenek diéta preferenciák megadva. Kérlek, állítsd be a preferenciáidat.", "OK");
                Console.WriteLine("[DEBUG] Nem található diéta preferencia a felhasználóhoz");
                return;
            }
            Console.WriteLine($"[DEBUG] Talált preferenciák: FoodIntolerance = {preferences.FoodIntolerance}, DislikedFoods = {preferences.DislikedFoods}, DietGoal = {preferences.DietGoal}, CookingTime = {preferences.CookingTime}");

            // Gyûjtemények törlése
            breakfastMeals.Clear();
            lunchMeals.Clear();
            dinnerMeals.Clear();
            recommendedMeals.Clear();

            // Ételintoleranciákhoz tartozó tiltott alapanyagok meghatározása
            var intoleranceIngredients = new Dictionary<string, List<string>>
            {
                { "dairy", new List<string> { "milk", "cheese", "yogurt", "butter", "cream" } },
                { "gluten", new List<string> { "pasta", "bread", "flour", "wheat", "barley" } },
                { "nuts", new List<string> { "almond", "peanut", "walnut", "cashew", "hazelnut" } },
                { "seafood", new List<string> { "fish", "shrimp", "crab", "lobster", "salmon", "tuna" } }
            };

            // Felhasználói preferenciák feldolgozása
            var foodIntolerances = preferences.FoodIntolerance?.ToLower() == "none" || string.IsNullOrWhiteSpace(preferences.FoodIntolerance)
                ? new List<string>()
                : preferences.FoodIntolerance.Split(',').Select(s => s.Trim().ToLower()).ToList();
            var dislikedFoods = preferences.DislikedFoods?.ToLower() == "none" || string.IsNullOrWhiteSpace(preferences.DislikedFoods)
                ? new List<string>()
                : preferences.DislikedFoods.Split(',').Select(s => s.Trim().ToLower()).ToList();
            var dietGoal = preferences.DietGoal?.ToLower();
            var maxCookingTime = preferences.CookingTime;

            Console.WriteLine($"[DEBUG] Feldolgozott preferenciák: FoodIntolerances = {string.Join(",", foodIntolerances)}, DislikedFoods = {string.Join(",", dislikedFoods)}, DietGoal = {dietGoal}, MaxCookingTime = {maxCookingTime}");


            // Kalóriakorlátok meghatározása a diéta célokhoz
            var calorieThresholds = new Dictionary<string, (int Min, int Max)>
            {
                { "bulk", (600, 2000) },
                { "lose fat", (50, 500) },
                { "maintain", (200, 1500) }
            };

            // Receptek szûrése az ajánlásokhoz
            foreach (var meal in meals)
            {
                bool isSuitable = true;

                // Recept részleteinek naplózása
                Console.WriteLine($"[DEBUG] Recept értékelése: {meal.Title}, Kalóriák = {meal.Calories}, Összetevõk = {(meal.Ingredients != null ? string.Join(", ", meal.Ingredients) : "null")}");

                // Ételintoleranciák ellenõrzése (csak ha meg van adva)
                if (foodIntolerances.Any())
                {
                    if (meal.Ingredients == null || !meal.Ingredients.Any())
                    {
                        isSuitable = false;
                        Console.WriteLine($"[DEBUG] Recept {meal.Title} kizárva: Összetevõk null vagy üresek");
                    }
                    else
                    {
                        foreach (var intolerance in foodIntolerances)
                        {
                            if (intoleranceIngredients.ContainsKey(intolerance))
                            {
                                var forbiddenIngredients = intoleranceIngredients[intolerance];
                                if (meal.Ingredients.Any(ing => forbiddenIngredients.Any(fi => ing?.ToLower().Contains(fi) == true)))
                                {
                                    isSuitable = false;
                                    Console.WriteLine($"[DEBUG] Recept {meal.Title} kizárva intolerancia miatt: {intolerance}");
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine($"[DEBUG] Recept {meal.Title} megfelel az intolerancia szûrésnek: {intolerance}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"[DEBUG] Nincs intolerancia kulcs a szótárban: {intolerance}");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"[DEBUG] Nincs ételintolerancia megadva, szûrés kihagyása");
                }

                // Nem kedvelt ételek ellenõrzése (csak ha meg van adva)
                if (isSuitable && dislikedFoods.Any())
                {
                    if (meal.Ingredients == null || !meal.Ingredients.Any())
                    {
                        isSuitable = false;
                        Console.WriteLine($"[DEBUG] Recept {meal.Title} kizárva: Összetevõk null vagy üresek");
                    }
                    else if (meal.Ingredients.Any(ing => dislikedFoods.Contains(ing?.ToLower())))
                    {
                        isSuitable = false;
                        Console.WriteLine($"[DEBUG] Recept {meal.Title} kizárva nem kedvelt étel miatt");
                    }
                }

                // Diéta cél (kalóriák) ellenõrzése
                if (isSuitable && !string.IsNullOrEmpty(dietGoal) && calorieThresholds.ContainsKey(dietGoal))
                {
                    var (minCal, maxCal) = calorieThresholds[dietGoal];
                    if (meal.Calories < minCal || meal.Calories > maxCal)
                    {
                        isSuitable = false;
                        Console.WriteLine($"[DEBUG] Recept {meal.Title} kizárva kalóriák miatt: {meal.Calories} nem a [{minCal}, {maxCal}] tartományban van");
                    }
                    else
                    {
                        Console.WriteLine($"[DEBUG] Recept {meal.Title} megfelel: Kalóriák {meal.Calories} a [{minCal}, {maxCal}] tartományban");
                    }
                }
                else if (isSuitable && !string.IsNullOrEmpty(dietGoal))
                {
                    Console.WriteLine($"[DEBUG] Nincs kalóriakorlát a dietGoal-hoz: {dietGoal}. Kalóriaszûrõ kihagyása.");
                }

                // Fõzési idõ ellenõrzése
                if (isSuitable && meal.CookingTime > maxCookingTime)
                {
                    isSuitable = false;
                }

                // Gyûjteményekhez adás
                if (string.IsNullOrEmpty(meal.AuthorName))
                {
                    meal.AuthorName = "Ismeretlen";
                }

                if (isSuitable)
                {
                    recommendedMeals.Add(meal);
                }

                switch (meal.Type?.ToUpper())
                {
                    case "BREAKFAST":
                        breakfastMeals.Add(meal);
                        break;
                    case "LUNCH":
                        lunchMeals.Add(meal);
                        break;
                    case "DINNER":
                        dinnerMeals.Add(meal);
                        break;
                    default:
                        Console.WriteLine($"[DEBUG] unknwn recept tipus: {meal.Type}");
                        break;
                }
            }

            // olda frissites
            BreakfastStack.ItemsSource = breakfastMeals;
            LunchStack.ItemsSource = lunchMeals;
            DinnerStack.ItemsSource = dinnerMeals;
            RecommendedStack.ItemsSource = recommendedMeals;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hiba", $"Nem sikerült betölteni a recepteket: {ex.Message}", "OK");
        }
    }

    private async void AddMealButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddMealPage());
    }

    private async Task ViewRecipe(Meal meal)
    {
        await Navigation.PushAsync(new RecipeDetailPage(meal));
    }
}