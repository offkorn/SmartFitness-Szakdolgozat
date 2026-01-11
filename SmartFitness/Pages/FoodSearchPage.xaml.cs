using SmartFitness.Models;
using System.Collections.ObjectModel;

namespace SmartFitness.Pages;

public partial class FoodSearchPage : ContentPage
{
    private List<Meal> _allSourceMeals; 
    private List<Meal> _currentFilteredList; 
    public ObservableCollection<Meal> DisplayedMeals { get; set; } = new(); 

    private const int BatchSize = 15; // Egyszerre ennyit töltünk be
    private bool _isLoading = false;

    public FoodSearchPage(List<Meal> meals)
    {
        InitializeComponent();
        _allSourceMeals = meals;
        _currentFilteredList = new List<Meal>(_allSourceMeals); 

        ResultsCollectionView.ItemsSource = DisplayedMeals;

        LoadNextBatch();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // SearchEntry.Focus(); 
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        string term = e.NewTextValue?.ToLower()?.Trim();

        ClearButton.IsVisible = !string.IsNullOrEmpty(term);

        
        if (string.IsNullOrWhiteSpace(term))
        {
           
            _currentFilteredList = new List<Meal>(_allSourceMeals);
        }
        else
        {
           
            _currentFilteredList = _allSourceMeals.Where(m =>
                (!string.IsNullOrEmpty(m.Title) && m.Title.ToLower().Contains(term)) ||
                (m.Ingredients != null && m.Ingredients.Any(i => i != null && i.ToLower().Contains(term)))
            ).ToList();
        }

        // Képernyõ lista ürítése és újratöltése az elejérõl
        DisplayedMeals.Clear();
        LoadNextBatch();
    }

    // ha a user legörget az aljára
    private void OnLoadMoreItems(object sender, EventArgs e)
    {
        LoadNextBatch();
    }

    private void LoadNextBatch()
    {
        if (_isLoading) return;
        _isLoading = true;

        try
        {
            int currentCount = DisplayedMeals.Count;
            int totalCount = _currentFilteredList.Count;

            
            if (currentCount >= totalCount) return;

            
            int amountToTake = Math.Min(BatchSize, totalCount - currentCount);

            var nextBatch = _currentFilteredList.GetRange(currentCount, amountToTake);

            foreach (var meal in nextBatch)
            {
                DisplayedMeals.Add(meal);
            }
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void OnClearSearchClicked(object sender, EventArgs e)
    {
        SearchEntry.Text = string.Empty;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnResultTapped(object sender, TappedEventArgs e)
    {
        if (sender is Border border && border.BindingContext is Meal selectedMeal)
        {
            await Navigation.PushAsync(new RecipeDetailPage(selectedMeal));
        }
    }
}