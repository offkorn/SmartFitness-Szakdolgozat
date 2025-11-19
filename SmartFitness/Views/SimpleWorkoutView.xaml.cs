using SmartFitness.Helpers;
using SmartFitness.ViewModels;
using static System.Collections.Specialized.NameObjectCollectionBase;

namespace SmartFitness.Views;

public partial class SimpleWorkoutView : ContentView
{
    public SimpleWorkoutView()
    {
        InitializeComponent();
        
    }

    private void OnLocationChanged(object sender, CheckedChangedEventArgs e)
    {
        try
        {
            var radioButton = sender as RadioButton;
            if (radioButton?.IsChecked == true)
            {
                var viewModel = BindingContext as SimpleWorkoutViewModel;
                if (viewModel != null)
                {
                    viewModel.Location = radioButton.Value.ToString();
                    Console.WriteLine($"Location changed to: {viewModel.Location}");
                }
                else
                {
                    Console.WriteLine("OnLocationChanged: ViewModel is null");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"OnLocationChanged error: {ex.Message}");
        }
    }

    private void OnEquipmentChanged(object sender, CheckedChangedEventArgs e)
    {
        try
        {
            var checkBox = sender as CheckBox;
            var viewModel = BindingContext as SimpleWorkoutViewModel;
            if (viewModel == null)
            {
                Console.WriteLine("OnEquipmentChanged: ViewModel is null");
                return;
            }

            string equipment = checkBox switch
            {
                CheckBox cb when cb == MachineCheckBox => "Machine",
                CheckBox cb when cb == WeightsCheckBox => "Weights",
                CheckBox cb when cb == BodyweightCheckBox => "Bodyweight",
                _ => null
            };

            if (equipment != null)
            {
                if (e.Value && !viewModel.Equipment.Contains(equipment))
                {
                    viewModel.Equipment.Add(equipment);
                    Console.WriteLine($"Equipment added: {equipment}");
                }
                else if (!e.Value && viewModel.Equipment.Contains(equipment))
                {
                    viewModel.Equipment.Remove(equipment);
                    Console.WriteLine($"Equipment removed: {equipment}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"OnEquipmentChanged error: {ex.Message}");
        }
    }

    private void OnGoalChanged(object sender, CheckedChangedEventArgs e)
    {
        try
        {
            var checkBox = sender as CheckBox;
            var viewModel = BindingContext as SimpleWorkoutViewModel;
            if (viewModel == null)
            {
                Console.WriteLine("OnGoalChanged: ViewModel is null");
                return;
            }

            string goal = checkBox switch
            {
                CheckBox cb when cb == StrengthCheckBox => "Strength",
                CheckBox cb when cb == MuscleBuildCheckBox => "Muscle Build",
                CheckBox cb when cb == CardioCheckBox => "Cardio",
                CheckBox cb when cb == HealthierLifeCheckBox => "Healthier Life",
                _ => null
            };

            if (goal != null)
            {
                if (e.Value && !viewModel.Goals.Contains(goal))
                {
                    viewModel.Goals.Add(goal);
                    Console.WriteLine($"Goal added: {goal}");
                }
                else if (!e.Value && viewModel.Goals.Contains(goal))
                {
                    viewModel.Goals.Remove(goal);
                    Console.WriteLine($"Goal removed: {goal}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"OnGoalChanged error: {ex.Message}");
        }
    }
}
