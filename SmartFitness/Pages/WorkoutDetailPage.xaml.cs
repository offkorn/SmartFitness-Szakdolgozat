using SmartFitness.ViewModels;
using Microsoft.Maui.Controls;

namespace SmartFitness.Pages;

public partial class WorkoutDetailPage : ContentPage
{
    public WorkoutDetailPage(WorkoutDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}