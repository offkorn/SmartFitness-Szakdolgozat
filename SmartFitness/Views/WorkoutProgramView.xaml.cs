using Microsoft.Maui.Controls;
using SmartFitness.ViewModels;

namespace SmartFitness.Views;

public partial class WorkoutProgramView : ContentView
{
    public WorkoutProgramView()
    {
        InitializeComponent();
        BindingContext = new WorkoutProgramViewModel();
    }

    private void OnLocationChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.IsChecked)
        {
            var viewModel = BindingContext as WorkoutProgramViewModel;
            viewModel?.SetLocation(radioButton.Value.ToString());
        }
    }

    private void OnEquipmentChanged(object sender, CheckedChangedEventArgs e)
    {
        var viewModel = BindingContext as WorkoutProgramViewModel;
        if (sender is CheckBox checkBox)
        {
            var equipment = checkBox == MachineCheckBox ? "Machine" :
                            checkBox == WeightsCheckBox ? "Weights" :
                            checkBox == BodyweightCheckBox ? "Bodyweight" : null;
            if (equipment != null)
            {
                viewModel?.UpdateEquipment(equipment, checkBox.IsChecked);
            }
        }
    }

    private async void SubmitButton_Clicked(object sender, EventArgs e)
    {
        var viewModel = BindingContext as WorkoutProgramViewModel;
        await viewModel?.SaveWorkoutPlan();
    }
}