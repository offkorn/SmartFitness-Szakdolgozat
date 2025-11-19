using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmartFitness.Models;

[Table("workouts")]
public class Workout : BaseModel
{
    [PrimaryKey("id", false)]
    public string Id { get; set; }

    [Column("title")]
    public string Title { get; set; }

    [Column("image_url")]
    public string ImageUrl { get; set; }

    [Column("location")]
    public string Location { get; set; } // "Gym" vagy "Home"

    [Column("equipment")]
    public List<string> Equipment { get; set; }

    [Column("duration_minutes")]
    public int DurationMinutes { get; set; }

    [Column("goals")]
    public List<string> Goals { get; set; }

    [Column("experience_level")]
    public string ExperienceLevel { get; set; }

    [Column("exercises")]
    public List<Exercise> Exercises { get; set; }

    [Column("created_by")]
    public string CreatedBy { get; set; } // UUID-hoz string

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("plan_id")]
    public string? PlanId { get; set; }

    [Column("day")]
    public string? Day { get; set; }

    public static implicit operator string?(Workout? v)
    {
        throw new NotImplementedException();
    }
}

public class Exercise : INotifyPropertyChanged
{
    private string _id;
    private string _name;
    private ObservableCollection<ExerciseSet> _sets;
    private ObservableCollection<Exercise> _subExercises;
    private bool _isMainExercise = true;

    public string Id
    {
        get => _id;
        set
        {
            _id = value;
            OnPropertyChanged();
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<ExerciseSet> Sets
    {
        get => _sets;
        set
        {
            _sets = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Exercise> SubExercises
    {
        get => _subExercises;
        set
        {
            _subExercises = value;
            OnPropertyChanged();
        }
    }

    public bool IsMainExercise
    {
        get => _isMainExercise;
        set
        {
            _isMainExercise = value;
            OnPropertyChanged();
        }
    }

    public Exercise()
    {
        Id = Guid.NewGuid().ToString();
        Sets = new ObservableCollection<ExerciseSet>();
        SubExercises = new ObservableCollection<Exercise>();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class ExerciseSet : INotifyPropertyChanged
{
    private string _id;
    private int _setNumber;
    private double _weight;
    private int _repetitions;

    public string Id
    {
        get => _id;
        set
        {
            _id = value;
            OnPropertyChanged();
        }
    }

    public int SetNumber
    {
        get => _setNumber;
        set
        {
            _setNumber = value;
            OnPropertyChanged();
        }
    }

    public double Weight
    {
        get => _weight;
        set
        {
            _weight = value;
            OnPropertyChanged();
        }
    }

    public int Repetitions
    {
        get => _repetitions;
        set
        {
            _repetitions = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}