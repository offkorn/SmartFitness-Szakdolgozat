using SmartFitness.Models;
using System.Collections.ObjectModel;

namespace SmartFitness.Pages;

public partial class SavedMealsPage : ContentPage
{

    private List<Meal> _allSourceMeals; 
    private List<Meal> _currentFilteredList; 
    public ObservableCollection<Meal> DisplayedMeals { get; set; } = new(); 

    private const int BatchSize = 15; 
    private bool _isLoading = false;


    public SavedMealsPage(List<Meal> meals)
	{
        InitializeComponent();
        _allSourceMeals = meals;
        _currentFilteredList = new List<Meal>(_allSourceMeals); // Kezdetben minden recept a listában van

        ResultsCollectionView.ItemsSource = DisplayedMeals;

        // Azonnal betöltjük az elsõ adagot (így üres keresõnél is látszanak)
        LoadNextBatch();
    }

   

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

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

    private async void OnResultTapped(object sender, TappedEventArgs e)
    {
       
        if (sender is Element element && element.BindingContext is Meal selectedMeal)
        {
            
            await Navigation.PushAsync(new RecipeDetailPage(selectedMeal));
        }
    }


}